using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ViridaxGameStudios
{
    [Serializable]
    public class CharacterStats
    {
        #region Instance Variables
        private int m_Level;
        private float m_StatsMultiplier;
        private int m_StatLevelUpIncrease;
        //Base Stats
        private int m_Strength;
        private int m_Intelligence;
        private int m_Faith;
        //Extension Stats
        private float m_MaxHitPoints;
        private float m_MovementSpeed;
        private float m_AttackDamage;
        private float m_AttackRange;
        private float m_DamageAngle;

        public int Level { get { return m_Level; } }
        public float StatsMultiplier { get { return m_StatsMultiplier; } set { m_StatsMultiplier = value; } }
        public int StatsLevelUpIncrease { get { return m_StatLevelUpIncrease; } set { m_StatLevelUpIncrease = value; } }
        public int Strength { get { return m_Strength; } }
        public int Intelligence { get { return m_Intelligence; } }
        public int Faith { get { return m_Faith; } }
        public float MaxHitPoints { get { return m_MaxHitPoints; } }
        public float MovementSpeed { get { return m_MovementSpeed; } set { m_MovementSpeed = value; } }
        public float AttackDamage { get { return m_AttackDamage; } }
        public float AttackRange { get { return m_AttackRange; } set { m_AttackRange = value; } }
        public float DamageAngle { get { return m_DamageAngle; } set { m_DamageAngle = value; } }
        #endregion

        public CharacterStats(float m_StatsMultiplier, int m_StatLevelUpIncrease, int m_Strength, int m_Intelligence, int m_Faith, float m_MovementSpeed)
        {
            m_Level = 0;
            if (m_StatsMultiplier <= 0)
                m_StatsMultiplier = 1.75f;
            if(m_StatLevelUpIncrease <= 0)
            {
                this.m_StatLevelUpIncrease = 5;
            }
            this.m_StatsMultiplier = m_StatsMultiplier;
            this.m_Strength = m_Strength;
            this.m_Intelligence = m_Intelligence;
            this.m_Faith = m_Faith;
            this.m_MovementSpeed = m_MovementSpeed;
            

            m_MaxHitPoints = Strength * Intelligence * Faith;
            m_AttackDamage = StatsMultiplier * (Strength + Intelligence + Faith);
        }

        public void LevelUp()
        {
            //
            //Method Name : void LevelUp()
            //Purpose     : This method increases the level by one and adjusts the Base Stats accordingly.
            //Re-use      : none
            //Input       : float damage
            //Output      : none
            //
            m_Level++;
            m_Strength = Strength + m_StatLevelUpIncrease;
            m_Intelligence = Intelligence + m_StatLevelUpIncrease;
            m_Faith = Faith + m_StatLevelUpIncrease;
        }//end LevelUp()
    }//end class
}

