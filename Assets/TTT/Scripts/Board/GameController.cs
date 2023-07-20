using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace TTT.Board
{
    // 游戏控制器

    public class GameController : MonoBehaviour
    {
        [SerializeField] private Button newGameBtn;
        [SerializeField] private Button exitGameBtn;
        [SerializeField] public Text winnerText,playerIdText;

        public static int round;
        public static int moves;    // 总计步数
        public static bool gameOver,roundEnded=false;
        public static string winnertext="";

        public static int currentplayerId=1;

        private void Awake() => AddListeners();

        private void AddListeners()
        {
            newGameBtn.onClick.AddListener(NewGame);
            exitGameBtn.onClick.AddListener(ExitGame);
        }

        private void NewGame()
        {
            winnerText.gameObject.SetActive(false);
            winnerText.text="";
            playerIdText.text="玩家"+currentplayerId+"执棋中";
            gameOver = false;
            SceneManager.LoadScene("TTT");
        }

        private void ExitGame()
        {
            winnerText.gameObject.SetActive(false);
            winnerText.text="";
            playerIdText.text="";
            moves = 0;
            round = 0;
            gameOver = false;
            SceneManager.LoadScene("MainMenu");
        }

        private void Update()
        {
            if(!roundEnded){
                if(currentplayerId<0){
                    playerIdText.text="";
                    roundEnded=true;
                }
                else{
                playerIdText.text="玩家"+currentplayerId+"执棋中";
                Debug.Log("玩家"+currentplayerId+"执棋中");
                roundEnded=true;
                }
            }
            if (gameOver || moves >= 9)
                PrepareExit();
        }

        private void PrepareExit()
        {
            DeclareWinner();
            gameOver = true;
            moves = 0;
            round = 0;
            newGameBtn.gameObject.SetActive(true);
        }

        private void DeclareWinner(){
            if(winnertext==""){
                winnerText.text=("平局");
            }
            else{
                winnerText.text=("赢家是："+winnertext);
            }
            winnerText.gameObject.SetActive(true);
            GameController.gameOver = true;
        }
    }
}