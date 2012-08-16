using System.Collections.Generic;
using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CStats : Component
    {
        private Dictionary<string, int> _baseStats;

        private int BaseMaxHealth { get { return 50 + Strength * 2 + Endurance * 9 + Luck; } }
        private int BaseMaxActionPoints { get { return 50 + Agility * 9 + Endurance * 2 + Strength * 2 + Luck; } }
        private int BaseMeleeDamage { get { return 10 + Strength * 3 + Endurance / 2 + Luck; } }
        private int BaseAccuracy { get { return 10 + Perception * 7 + Strength / 2 + Endurance / 2 + Agility + Luck; } }

        private int _addMaxHealth;
        private int _addMaxActionPoints;
        private int _addMeleeDamage;

        public CStats(int mStrength, int mPerception, int mEndurance, int mCharisma, int mIntelligence, int mAgility, int mLuck)
        {
            _baseStats.Add("baseStrength", 0);
            _baseStats.Add("basePerception", 0);
            _baseStats.Add("baseEndurance", 0);
            _baseStats.Add("baseCharisma", 0);
            _baseStats.Add("baseIntelligence", 0);
            _baseStats.Add("baseAgility", 0);
            _baseStats.Add("baseLuck", 0);

            _baseStats.Add("addStrength", 0);
            _baseStats.Add("addPerception", 0);
            _baseStats.Add("addEndurance", 0);
            _baseStats.Add("addCharisma", 0);
            _baseStats.Add("addIntelligence", 0);
            _baseStats.Add("addAgility", 0);
            _baseStats.Add("addLuck", 0);

            SetBaseStat("baseStrength", mStrength);
            SetBaseStat("basePerception", mPerception);
            SetBaseStat("baseEndurance", mEndurance);
            SetBaseStat("baseCharisma", mCharisma);
            SetBaseStat("baseIntelligence", mIntelligence);
            SetBaseStat("baseAgility", mAgility);
            SetBaseStat("baseLuck", mLuck);
        }

        public int Strength { get { return _baseStats["baseStrength"] + _baseStats["addStrength"]; } }
        public int Perception { get { return _baseStats["basePerception"] + _baseStats["addPerception"]; } }
        public int Endurance { get { return _baseStats["baseEndurance"] + _baseStats["addEndurance"]; } }
        public int Charisma { get { return _baseStats["baseCharisma"] + _baseStats["addCharisma"]; } }
        public int Intelligence { get { return _baseStats["baseIntelligence"] + _baseStats["addIntelligence"]; } }
        public int Agility { get { return _baseStats["baseAgility"] + _baseStats["addAgility"]; } }
        public int Luck { get { return _baseStats["baseLuck"] + _baseStats["addLuck"]; } }

        public int MaxHealth { get { return BaseMaxHealth + _addMaxHealth; } }
        public int MaxActionPoints { get { return BaseMaxActionPoints + _addMaxActionPoints; } }
        public int MeleeDamage { get { return BaseMeleeDamage + _addMeleeDamage; } }

        public int GetBaseStat(string mStatName) { return _baseStats[mStatName]; }
        public void SetBaseStat(string mStatName, int mValue) { _baseStats[mStatName] = mValue; }
    }
}