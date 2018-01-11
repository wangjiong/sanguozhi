using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityMesh : MaskableGraphic {
    static float M = 0.57735026919f;
    static float SCALE = 50;
    VertexHelper mVertexHelper = new VertexHelper();

    public float v1 = 1;
    public float v2 = 1;
    public float v3 = 1;
    public float v4 = 1;
    public float v5 = 1;

    protected override void OnPopulateMesh(VertexHelper vh) {
        vh.Clear();

        vh.AddVert(new Vector3(0, 0) * SCALE, new Color(31 / 255f, 50 / 255f, 56 / 255f, 0.5f), Vector2.zero);
        vh.AddVert(new Vector3(0, 2 * M) * SCALE * v1, new Color(167 / 255f, 113 / 255f, 127 / 255f, 0.5f), Vector2.zero);
        vh.AddVert(new Vector3(1, M) * SCALE * v2, new Color(108 / 255f, 88 / 255f, 136 / 255f, 0.5f), Vector2.zero);
        vh.AddVert(new Vector3(M, -1 * 0.7f) * SCALE * v3, new Color(160 / 255f, 160 / 255f, 73 / 255f, 0.5f), Vector2.zero);
        vh.AddVert(new Vector3(-M, -1 * 0.7f) * SCALE * v4, new Color(64 / 255f, 162 / 255f, 130 / 255f, 0.5f), Vector2.zero);
        vh.AddVert(new Vector3(-1, M) * SCALE * v5, new Color(85 / 255f, 139 / 255f, 168 / 255f, 0.5f), Vector2.zero);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(0, 2, 3);
        vh.AddTriangle(0, 3, 4);
        vh.AddTriangle(0, 4, 5);
        vh.AddTriangle(0, 5, 1);
    }
}