namespace ReFMGame.GameHelper
{
    class SmartFramerate
    {
        double currentFrametimes;
        readonly double weight;
        readonly int numerator;

        public double Framerate
        {
            get
            {
                return numerator / currentFrametimes;
            }
        }

        public SmartFramerate(int oldFrameWeight)
        {
            numerator = oldFrameWeight;
            weight = oldFrameWeight / (oldFrameWeight - 1d);
        }

        public void Update(double timeSinceLastFrame)
        {
            currentFrametimes /= weight;
            currentFrametimes += timeSinceLastFrame;
        }
    }
}
