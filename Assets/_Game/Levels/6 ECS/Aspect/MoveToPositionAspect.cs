
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public readonly partial struct MoveToPositionAspect : IAspect
{
    private readonly RefRW<LocalTransform> _transformAspect;
    private readonly RefRO<TargetPositionComponent> _targetPosition;

    public void Move(float time)
    {
        var (pos, rot) = CalculatePosBurst(_targetPosition.ValueRO.Value.y, time);

        _transformAspect.ValueRW.Rotate(rot);
        _transformAspect.ValueRW.Translate(pos);
    }
    
    private (float3 position, quaternion rotation) CalculatePosBurst(float yOffset, float time)
    {
        // // 로컬 포지션 및 회전 계산 로직을 여기에 구현
        // // position과 rotation을 업데이트
        //
        // float temp = y * time;
        //
        // // 예시로 임의의 회전과 위치를 계산
        // float4 rotation = new float4(0, temp, 0, 0);
        // float3 position = new float3(0, temp, 0);
        //
        // return (position, rotation);
        var pos = _transformAspect.ValueRW.Position;
        
        var t = math.unlerp(yOffset, SceneTools.BURST_HEIGHT_SCALE + yOffset, pos.y);
        pos.y = SceneTools.BURST_HEIGHT_SCALE * noise.cnoise(new float2(pos.x * SceneTools.NOISE_SCALE + time, pos.z * SceneTools.NOISE_SCALE + time)) +
                yOffset * SceneTools.DEPTH_OFFSET;
        var rot = math.nlerp(quaternion.identity, SceneTools.RotGoal, t);
        return (pos, rot);
    }
}