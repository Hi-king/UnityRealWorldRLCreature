using MotionGenerator;
using MotionGenerator.Entity.Soul;

namespace MapHack
{
    public class AvoidCrashSoul: Soul
    {
        public override float Reward(State lastState, State nowState)
        {
            if (nowState.ContainsKey(CrashSensor.Key) && nowState.GetAsDouble(CrashSensor.Key) > 0.9)
            {
                return -10;
            }

            return 0;
        }
    }
}