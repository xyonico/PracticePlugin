using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PracticePlugin
{
    class ScoreListItem
    {
        public int baseScore = 0;
        public int previousScore = 0;
        public int multiplier = 0;
        public int multiplierIncreaseProgress = 0;
        public int multiplierIncreaseMaxProgress = 0;
        public int combo = 0;
        public int maxCombo = 0;
        public bool playerHeadWasInObstacle;
        public List<AfterCutScoreBuffer> afterCutScoreBuffers = new List<AfterCutScoreBuffer>();
        
        public ScoreListItem() { }
    }
}
