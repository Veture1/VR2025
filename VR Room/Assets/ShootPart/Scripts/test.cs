using UnityEngine;

public class TestResonancePillar : MonoBehaviour
{
    public ResonancePillar pillar;     // 手动拖拽柱子
    private float testY = 0f;          // 当前测试高度
    public float stepY = 0.25f;        // 每次上升的步长（用于模拟 8段柱子）

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 hitPoint = pillar.transform.position + Vector3.up * testY;
            pillar.RegisterHit(hitPoint);
            Debug.Log($"Test Hit at Y={testY:F2}");
            testY += stepY;

            if (testY > pillar.pillarHeight)
            {
                testY = 0f; // 重置测试
            }
        }
    }
}
