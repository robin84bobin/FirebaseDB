using UnityEngine;

namespace Assets.Scripts.UI.Windows
{
    public class Leaderboardwindow : MonoBehaviour {

        public static void Show ()
        {
            App.UI.Show(	"Leaderboardwindow");
        }
    }
}
