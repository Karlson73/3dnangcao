using UnityEngine;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    // ===== Trạng thái trả về của mỗi node trong Behaviour Tree =====
    public enum NodeState { Running, Success, Failure }

    // ===== Node gốc (tất cả node khác đều kế thừa) =====
    public abstract class Node
    {
        public abstract NodeState Evaluate(); // Hàm xử lý chính của node
    }

    // ===== SELECTOR: chọn node đầu tiên không Failure =====
    public class Selector : Node
    {
        List<Node> children;
        public Selector(List<Node> nodes) => children = nodes;

        public override NodeState Evaluate()
        {
            foreach (Node node in children)
            {
                NodeState state = node.Evaluate();
                if (state != NodeState.Failure)
                    return state; // Success hoặc Running thì dừng
            }
            return NodeState.Failure;
        }
    }

    // ===== SEQUENCE: chạy lần lượt, fail là dừng =====
    public class Sequence : Node
    {
        List<Node> children;
        public Sequence(List<Node> nodes) => children = nodes;

        public override NodeState Evaluate()
        {
            foreach (Node node in children)
            {
                NodeState state = node.Evaluate();
                if (state != NodeState.Success)
                    return state; // Running hoặc Failure thì dừng
            }
            return NodeState.Success;
        }
    }

    // ===== DỮ LIỆU ENEMY =====
    [Header("Target")]
    public Transform player;          // Player để enemy theo dõi

    [Header("Ranges")]
    public float detectRange = 7f;    // Phạm vi phát hiện player
    public float attackRange = 2f;    // Phạm vi tấn công

    [Header("Movement")]
    public float moveSpeed = 2.5f;    // Tốc độ di chuyển

    [Header("Patrol Area")]
    public float patrolRadius = 4f;   // Bán kính vùng tuần tra
    private Vector3 patrolCenter;     // Tâm vùng tuần (vị trí ban đầu)
    private Vector3 patrolTarget;     // Điểm tuần ngẫu nhiên

    private Node root;                // Node gốc của Behaviour Tree

    void Start()
    {
        patrolCenter = transform.position; // Lưu vị trí ban đầu
        SetNewPatrolTarget();              // Tạo điểm tuần đầu tiên

        // ===== XÂY DỰNG BEHAVIOUR TREE =====
        root = new Selector(new List<Node>
        {
            // Ưu tiên 1: ĐÁNH
            new Sequence(new List<Node>
            {
                new PlayerInAttackRange(this),
                new Attack()
            }),

            // Ưu tiên 2: ĐUỔI
            new Sequence(new List<Node>
            {
                new PlayerInDetectRange(this),
                new Chase(this)
            }),

            // Ưu tiên 3: QUAY VỀ VÙNG TUẦN
            new Sequence(new List<Node>
            {
                new EnemyOutsidePatrolArea(this),
                new ReturnToPatrolCenter(this)
            }),

            // Ưu tiên 4: TUẦN TRA
            new Patrol(this)
        });
    }

    void Update()
    {
        root.Evaluate(); // Cập nhật AI mỗi frame
    }

    // ===== TẠO ĐIỂM TUẦN NGẪU NHIÊN TRONG PHẠM VI =====
    void SetNewPatrolTarget()
    {
        Vector2 random = Random.insideUnitCircle * patrolRadius;
        patrolTarget = patrolCenter + new Vector3(random.x, 0, random.y);
    }

    // ===== NODE ĐIỀU KIỆN: PLAYER TRONG TẦM PHÁT HIỆN =====
    class PlayerInDetectRange : Node
    {
        EnemyAI e;
        public PlayerInDetectRange(EnemyAI enemy) => e = enemy;

        public override NodeState Evaluate()
        {
            return Vector3.Distance(e.transform.position, e.player.position) <= e.detectRange
                ? NodeState.Success : NodeState.Failure;
        }
    }

    // ===== NODE ĐIỀU KIỆN: PLAYER TRONG TẦM ĐÁNH =====
    class PlayerInAttackRange : Node
    {
        EnemyAI e;
        public PlayerInAttackRange(EnemyAI enemy) => e = enemy;

        public override NodeState Evaluate()
        {
            return Vector3.Distance(e.transform.position, e.player.position) <= e.attackRange
                ? NodeState.Success : NodeState.Failure;
        }
    }

    // ===== HÀNH ĐỘNG: ĐUỔI PLAYER =====
    class Chase : Node
    {
        EnemyAI e;
        public Chase(EnemyAI enemy) => e = enemy;

        public override NodeState Evaluate()
        {
            e.transform.position = Vector3.MoveTowards(
                e.transform.position,
                e.player.position,
                e.moveSpeed * Time.deltaTime
            );
            return NodeState.Running;
        }
    }

    // ===== HÀNH ĐỘNG: TẤN CÔNG =====
    class Attack : Node
    {
        float cooldown = 1f;   // Thời gian hồi đánh
        float lastTime;

        public override NodeState Evaluate()
        {
            if (Time.time - lastTime > cooldown)
            {
                Debug.Log("Enemy ATTACK!");
                lastTime = Time.time;
            }
            return NodeState.Running;
        }
    }

    // ===== ĐIỀU KIỆN: ENEMY RA NGOÀI VÙNG TUẦN =====
    class EnemyOutsidePatrolArea : Node
    {
        EnemyAI e;
        public EnemyOutsidePatrolArea(EnemyAI enemy) => e = enemy;

        public override NodeState Evaluate()
        {
            return Vector3.Distance(e.transform.position, e.patrolCenter) > e.patrolRadius
                ? NodeState.Success : NodeState.Failure;
        }
    }

    // ===== HÀNH ĐỘNG: QUAY VỀ TÂM VÙNG TUẦN =====
    class ReturnToPatrolCenter : Node
    {
        EnemyAI e;
        public ReturnToPatrolCenter(EnemyAI enemy) => e = enemy;

        public override NodeState Evaluate()
        {
            if (Vector3.Distance(e.transform.position, e.patrolCenter) < 0.1f)
                return NodeState.Success;

            e.transform.position = Vector3.MoveTowards(
                e.transform.position,
                e.patrolCenter,
                e.moveSpeed * Time.deltaTime
            );
            return NodeState.Running;
        }
    }

    // ===== HÀNH ĐỘNG: TUẦN TRA TRONG PHẠM VI =====
    class Patrol : Node
    {
        EnemyAI e;
        public Patrol(EnemyAI enemy) => e = enemy;

        public override NodeState Evaluate()
        {
            e.transform.position = Vector3.MoveTowards(
                e.transform.position,
                e.patrolTarget,
                e.moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(e.transform.position, e.patrolTarget) < 0.2f)
                e.SetNewPatrolTarget();

            return NodeState.Running;
        }
    }
}