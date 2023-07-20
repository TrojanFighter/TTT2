using UnityEngine;

namespace TTT.Board
{
    // 人类玩家交互棋盘行为

    public class HumanPlayer : MonoBehaviour
    {
        public static int playerId = 0;
        public static bool selfPlay;    // 人类对弈时 PlayerAI 的 playerId = -1

        public static int row;          // 玩家最近下棋的位置
        public static int col;

        private bool[,] board;
        private Transform boardTransform;

        private void Start()
        {
            boardTransform = BoardCreator.Instance.transform;
            board = (playerId == 0) ? BoardCreator.circleBoard : BoardCreator.crossBoard;
        }

        private void Update()
        {
            if (!GameController.gameOver)
                if (GameController.round == playerId)
                    if (Input.GetMouseButtonDown(0))
                        PlacePawm();
        }

        private void PlacePawm()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
            {
                if (hitInfo.collider.CompareTag("Tile"))
                {
                    col = Mathf.RoundToInt(hitInfo.point.x) + 1; 
                    row = 1 - Mathf.RoundToInt(hitInfo.point.y);

                    board[row, col] = true;

                    GameController.moves++;

                    GameController.round = selfPlay ? playerId : (++GameController.round) % 2;
                    int no = selfPlay ? GameController.moves % 2 : playerId;

                    hitInfo.collider.transform.GetChild(no).gameObject.SetActive(true);
                    hitInfo.collider.enabled = false;

                    GameController.currentplayerId=GameController.moves%2+1;
                    GameController.roundEnded=false;

                    SoundManager.GetSoundEffect(1, 0.5f);

                    if (selfPlay)                          
                        if (CheckIfOver())                 
                        {
                            GameController.winnertext="玩家"+(2-no);//取胜玩家编号
                            GameController.gameOver = true;
                            //GameController.DeclareWinner();
                            SoundManager.GetSoundEffect(0, 1f, 0.5f);
                        }
                        else if(GameController.gameOver || GameController.moves >= 9)
                        {
                            GameController.winnertext="";
                            GameController.gameOver = true;
                            //GameController.DeclareWinner();
                            SoundManager.GetSoundEffect(0, 1f, 0.5f);
                        }
                }
            }
        }

        //检查AI是否胜利
        private bool CheckIfOver()
        {
            for (int k = 0; k < 2; k++)    
            {
                int amount3 = 0;
                int amount4 = 0;

                for (int i = 0; i < 3; i++)
                {
                    int amount1 = 0;
                    int amount2 = 0;

                    for (int j = 0; j < 3; j++)
                    {
                        if (boardTransform.GetChild(j + 3 * i).GetChild(k).gameObject.activeSelf)   // 水平检查
                            amount1++;

                        if (boardTransform.GetChild(i + 3 * j).GetChild(k).gameObject.activeSelf)   // 垂直检查
                            amount2++;
                    }

                    if (boardTransform.GetChild(4 * i).GetChild(k).gameObject.activeSelf)           // 斜角
                        amount3++;

                    if (boardTransform.GetChild(2 * i + 2).GetChild(k).gameObject.activeSelf)       // 斜角
                        amount4++;

                    if (amount1 == 3 || amount2 == 3)
                        return true;
                }

                if (amount3 == 3 || amount4 == 3)
                    return true;
            }

            return false;
        }
    }
}