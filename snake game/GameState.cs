namespace snake_game
{
    
    public class GameState
    {
        private int SnakeLength = 3;
        public int Rows { get; }
        public int Cols { get; }
        public GridValue[,] Grid {  get; }
        public Direction Dir { get; private set; }
        public int Score {  get; private set; }
        public bool GameOver { get; private set; }

        private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();
        public readonly Random random = new Random();
        public GameState(int rows,int cols) 
        {
            Rows = rows; 
            Cols = cols;
            Grid=new GridValue[rows, cols];
            Dir = Direction.Right;
            AddSnake();
            AddFood();
        }
        private void AddSnake() 
        {
            int r = Rows / 2; //spawn in the middle
            for(int c=1;c<= SnakeLength; c++)
            {
                Grid[r, c] = GridValue.Snake;
                snakePositions.AddFirst(new Position(r, c));
            }
        }
        private IEnumerable<Position> EmptyPositions()
        {
            for(int r = 0;r<Rows;r++)
            {
                for (int c=0;c<Cols;c++)
                {
                    if (Grid[r,c]== GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }
        private void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPositions());
            if(empty.Count == 0) //if all of it taken by the snake
                return;
            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row,pos.Col]= GridValue.Food;

        }
        public Position HeadPosition()
        {
            return snakePositions.First.Value;
        }
        public Position TailPosition()
        {
            return snakePositions.Last.Value;
        }
        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }
        private void AddHead(Position pos)
        {
            snakePositions.AddFirst(pos);
            Grid[pos.Row,pos.Col]= GridValue.Snake;
        }
        private void RemoveTail()
        {
            Position Tail = snakePositions.Last.Value;
            Grid[Tail.Row, Tail.Col]= GridValue.Empty;
            snakePositions.RemoveLast();
        }
        public void ChangeDirection(Direction dir)
        {
            Dir = dir;
        }
        private bool OutSideGrid(Position pos)
        {
            return pos.Row <0 || pos.Col <0 ||pos.Row>=Rows || pos.Col>=Cols;
        }
        private GridValue WillHit(Position newPosHead)
        {
            if(OutSideGrid( newPosHead))
                return GridValue.Outside;
            if (newPosHead == TailPosition()) //Tail should move out of the way if in a circle
                return GridValue.Empty;

            return Grid[newPosHead.Row, newPosHead.Col];
        }
        public void Move()
        {
            Position newHeadPos = HeadPosition().Translate(Dir);
            GridValue hit = WillHit(newHeadPos);
            if (hit == GridValue.Outside || hit == GridValue.Snake)
                GameOver = true;
            else if( hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }else if (hit == GridValue.Food)
            {
                AddHead(newHeadPos);
                Score++;
                AddFood();
            }
        }
    }
}
