namespace KSPMouseInterface
{
    public class Size
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public bool IsEmpty
        {
            get
            {
                return (Width == 0 && Height == 0);
            }
        }


        public Size(int width = 0, int height = 0)
        {
            Width = width;
            Height = height;
        }


        public override string ToString()
        {
            return string.Format("(Width = {0}, Height = {1})", Width, Height);
        }
    }
}
