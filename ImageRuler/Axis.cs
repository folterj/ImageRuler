
namespace ImageRuler
{
    class Axis
    {
        public double image0 = 0;
        public double value0 = 0;
        public bool set0 = false;

        public double image1 = 0;
        public double value1 = 0;
        public bool set1 = false;

        public void reset()
        {
            image0 = 0;
            value0 = 0;
            set0 = false;

            image1 = 0;
            value1 = 0;
            set1 = false;
        }

        public void setStart(double image, double value)
        {
            image0 = image;
            value0 = value;
            set0 = true;
        }

        public void setEnd(double image, double value)
        {
            image1 = image;
            value1 = value;
            set1 = true;
        }

        public double getValue(double image)
        {
            double norm;

            if (isSet())
            {
                norm = (image - image0) / (image1 - image0);
                return value0 + norm * (value1 - value0);
            }
            return 0;
        }

        public bool isSet()
        {
            return (set0 && set1);
        }

    }
}
