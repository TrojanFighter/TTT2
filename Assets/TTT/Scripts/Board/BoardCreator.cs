using UnityEngine;

namespace TTT.Board
{
    // 棋盘管理

    public class BoardCreator : MonoBehaviour
    {
        [SerializeField] private GameObject tilePrefab;

        public static bool[,] circleBoard;       // tic-tac-toe bit arrays
        public static bool[,] crossBoard;

        public static BoardCreator Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            circleBoard = new bool[3, 3];
            crossBoard = new bool[3, 3];
        }

        private void Start() => GenerateBoard();

        private void GenerateBoard()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    Vector3 pos = new Vector3(j - 1, 1 - i, 0);
                    GameObject go = Instantiate(tilePrefab, pos, Quaternion.identity, transform);   //.name = "Tile_" + i + "_" + j;
                    go.transform.localScale = new Vector3(0.95f, 0.95f, 1);

                }
        }
    }
}