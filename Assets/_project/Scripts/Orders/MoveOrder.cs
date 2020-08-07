namespace FeedTheBaby
{
    public class MoveOrder : Order
    {
        float x;
        float y;

        public MoveOrder(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
}