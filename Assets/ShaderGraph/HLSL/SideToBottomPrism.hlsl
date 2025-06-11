void SideToBottomPrism_float(
    float3 Position,
    float3 RectTop0, float3 RectTop1, float3 RectTop2, float3 RectTop3,
    float3 RectBot0, float3 RectBot1, float3 RectBot2, float3 RectBot3,
    out float3 Output)
{
    // UV空間のように、x,z を 0~1 に正規化 (Box範囲が1x1x1想定)
    float3 p = Position + 0.5; // [-0.5, 0.5] → [0, 1]

    // 上面と下面の四角形を線形補間で結ぶ（8頂点定義 → 複数パネル）
    float3 top = lerp(
        lerp(RectTop0, RectTop1, p.z),
        lerp(RectTop3, RectTop2, p.z),
        p.y
    );

    float3 bot = lerp(
        lerp(RectBot0, RectBot1, p.z),
        lerp(RectBot3, RectBot2, p.z),
        p.y
    );

    // p.x で位置を補間（右面から底面への拡がり）
    Output = lerp(top, bot, p.x);
}
