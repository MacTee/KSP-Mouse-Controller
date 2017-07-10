namespace KSPMouseInterface
{
    public class Margin
    {
        public int Left { get; set; }
        public int Right { get; set; }
        public int Top { get; set; }
        public int Bottom { get; set; }
        public bool IsEmpty
        {
            get
            {
                return (Left == 0 && Right == 0 && Top == 0 && Bottom == 0);
            }
        }


        public Margin(int left = 0, int right = 0, int top = 0, int bottom = 0)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }


        public override string ToString()
        {
            return string.Format("(Left = {0}, Right = {1}, Top = {2}, Right = {3})", Left, Right, Top, Right);
        }
    }
}