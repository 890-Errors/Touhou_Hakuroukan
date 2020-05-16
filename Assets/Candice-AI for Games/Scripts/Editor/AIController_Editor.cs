using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ViridaxGameStudios.AI
{
    [CustomEditor(typeof(AIController))]
    public class AIController_Editor : Editor
    {
        #region variables
        private AIController character;
        private SerializedObject soTarget;
        string[] arrAITypes = { "Aggressive", "Passive"};
        string[] arrAttackTypes = {"Melee", "Range"};
        string[] arrTabs = { "AI Settings", "Stats", "Relationships", "Movement", "Combat", "Key Mapping" };
        string[] arrMoveType = {"Basic", "Obstacle Avoidance", "Pathfind"};
        string[] arrMoveType2D = { "Basic"};
        string[] arrPathfindSource = { "Candice", "Unity NavMesh" };
        SerializedProperty attackProjectile;
        SerializedProperty cam;
        private int tabIndex;
        private int enemyTagCount;
        private int patrolPointCount;
        private int allyTagCount;
        GUIStyle guiStyle = new GUIStyle();
        bool showPatrolPoints = false;
        bool showAllyTags = false;
        bool showEnemyTags = false;
        

        #endregion

        #region Main Methods
        void OnEnable()
        {
            //Store a reference to the AI Controller script
            character = (AIController)target;
            soTarget = new SerializedObject(character);
            attackProjectile = soTarget.FindProperty("attackProjectile");
            cam = soTarget.FindProperty("cam");
            
            guiStyle.fontSize = 14;
            guiStyle.fontStyle = FontStyle.Bold;


        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;
            Texture2D image = (Texture2D)Resources.Load("LogoScaled");
            GUIContent label = new GUIContent(image);

            
            GUILayout.Label(label);
            EditorGUI.BeginChangeCheck();
            //tabIndex = GUILayout.Toolbar(tabIndex, arrTabs);
            tabIndex = GUILayout.SelectionGrid(tabIndex, arrTabs, 3);
            switch (tabIndex)
            {
                case 0:
                    break;
                case 1:
                    break;
            }
            if (EditorGUI.EndChangeCheck())
            {
                GUI.FocusControl(null);
            }
            GUILayout.Space(8);
            GUILayout.BeginVertical("box");
            EditorGUI.BeginChangeCheck();
            switch (tabIndex)
            {
                case 0:
                    DrawAISettingGUI();
                    break;
                case 1:
                    DrawStatsGUI();

                    break;
                case 2:
                    
                    DrawRelationshipGUI();
                    break;
                case 3:
                    DrawMovementGUI();
                    break;
                case 4:
                    DrawCombatGUI();
                    break;
                case 5:
                    DrawKeyMapGUI();
                    break;
            }
            if (EditorGUI.EndChangeCheck())
            {

            }
            GUILayout.EndVertical();

        }
        void DrawRelationshipGUI()
        {
            
            GUIContent label = new GUIContent("Allies:", "All the Tags that the character will consider as an ally. NOTE: The default reaction is to follow.");
            EditorGUILayout.LabelField(label, guiStyle);
            showAllyTags = EditorGUILayout.Foldout(showAllyTags, label);
            if (showAllyTags)
            {
                allyTagCount = character.allyTags.Count;
                allyTagCount = EditorGUILayout.IntField("Size: ", allyTagCount);

                if (allyTagCount != character.allyTags.Count)
                {
                    int i = 0;
                    while (allyTagCount > character.allyTags.Count)
                    {
                        character.allyTags.Add("Ally" + i);
                        i++;
                    }
                    while (allyTagCount < character.allyTags.Count)
                    {
                        character.allyTags.RemoveAt(character.allyTags.Count - 1);
                    }
                }

                for (int i = 0; i < character.allyTags.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Element " + i);
                    string tag = "";
                    tag = EditorGUILayout.TagField(character.allyTags[i]);

                    if (character.enemyTags.Contains(tag))
                    {
                        EditorUtility.DisplayDialog("Basic AI Controller", "Tag '" + tag + "' already added to enemy tags", "OK");
                    }
                    else
                    {
                        character.allyTags[i] = tag;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }
            }
            //Enemy Relationships
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            label = new GUIContent("Enemies", "All the Tags that the character will consider as an enemy. NOTE: The default reaction is to attack.");
            EditorGUILayout.LabelField(label, guiStyle);
            showEnemyTags = EditorGUILayout.Foldout(showEnemyTags, label);
            if (showEnemyTags)
            {
                enemyTagCount = character.enemyTags.Count;
                enemyTagCount = EditorGUILayout.IntField("Size", enemyTagCount);

                if (enemyTagCount != character.enemyTags.Count)
                {
                    int i = 0;
                    while (enemyTagCount > character.enemyTags.Count)
                    {
                        character.enemyTags.Add("Enemy" + i);
                        i++;
                    }
                    while (enemyTagCount < character.enemyTags.Count)
                    {
                        character.enemyTags.RemoveAt(character.enemyTags.Count - 1);
                    }
                }

                for (int i = 0; i < character.enemyTags.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Element " + i);
                    string tag = "";
                    tag = EditorGUILayout.TagField(character.enemyTags[i]);
                    
                    if (character.allyTags.Contains(tag))
                    {
                        EditorUtility.DisplayDialog("Basic AI Controller", "Tag '" + tag + "' already added to ally tags", "OK");
                    }
                    else
                    {
                        character.enemyTags[i] = tag;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }
            }
        }
        void DrawStatsGUI()
        {
            GUIContent label = new GUIContent("Base Stats", "The base stats are used once the game runs to create the character object and do initial calculations, such as damages. They cannot be changed after runtime, except by calling LevelUp(). Only stats multiplier can be changed during runtime.");
            GUILayout.Label(label, guiStyle);
            label = new GUIContent("Stats Multiplier", "This is used to calculate the attack damage based on your Strength, Intelligence and Faith. This variable can be changed during runtime.");
            character.m_StatsMultiplier = EditorGUILayout.Slider(label, character.m_StatsMultiplier, 1f, 10);
            label = new GUIContent("Stats Level Up Increase", "This is used to increase each stat (strength intelligence, faith) when leveling up.");
            character.m_StatLevelUpIncrease = EditorGUILayout.IntSlider(label, character.m_StatLevelUpIncrease, 1, 100);
            label = new GUIContent("Strength", "This directly affects the HitPoints and physical damage.");
            character.m_Strength = (int)EditorGUILayout.IntField(label, character.m_Strength);
            label = new GUIContent("Intelligence", "This directly affects the HitPoints and mental damage.");
            character.m_Intelligence = (int)EditorGUILayout.IntField(label, character.m_Intelligence);
            label = new GUIContent("Faith", "This directly affects the HitPoints and spiritual damage.");
            character.m_Faith = (int)EditorGUILayout.IntField(label, character.m_Faith);


            EditorGUILayout.Space();
            label = new GUIContent("Attack Damage", "The characters attack damage, based on the base stats and stats multiplier. NOTE: This value cannot be changed and is completely dependent on the base stats and stat multiplier.");
            //GUILayout.Label(label, guiStyle);
            EditorGUILayout.FloatField(label, character.attackDamage);
            EditorGUILayout.Space();
        }
        void DrawKeyMapGUI()
        {
            GUIContent label = new GUIContent("Click to Move", "If selected, click the area to move the agent. If disabled, normal keys will be used (e.g WASD and arrow keys)");
            character.clickToMove = EditorGUILayout.Toggle(label, character.clickToMove);      
            character.keyAttack = (KeyCode)EditorGUILayout.EnumFlagsField("Attack 1", character.keyAttack);
            character.keyAttack2 = (KeyCode)EditorGUILayout.EnumFlagsField("Attack 2", character.keyAttack2);
            character.special1 = (KeyCode) EditorGUILayout.EnumFlagsField("Special 1", character.special1);
            character.special2 = (KeyCode) EditorGUILayout.EnumFlagsField("Special 2", character.special2);
            character.special3 = (KeyCode) EditorGUILayout.EnumFlagsField("Special 3", character.special3);
        }
        void DrawAISettingGUI()
        {
            GUIContent label;

            label = new GUIContent("Agent ID", "The unique ID of the agent. Automatically generated at runtime.");
            EditorGUILayout.TextField(label, character.agentID.ToString());

            label = new GUIContent("Behavior Tree", "");
            character.behaviorTree = (BehaviorTree) EditorGUILayout.ObjectField(label, character.behaviorTree, typeof(BehaviorTree), true);

            label = new GUIContent("Health Bar", "");
            character.healthBar = (HealthBarScript)EditorGUILayout.ObjectField(label, character.healthBar, typeof(HealthBarScript), true);
            label = new GUIContent("Is 3D", "Uncheck if this character is in 2D space.");
            
            character.is3D = EditorGUILayout.Toggle(label, character.is3D);
            label = new GUIContent("Faction", "The faction that this character belongs to.");
            character.faction = (Faction)EditorGUILayout.ObjectField(label, character.faction, typeof(Faction), true);
            label = new GUIContent("Detection Radius", "The radius which the character can detect other objects.");
            character.m_DetectionRadius = EditorGUILayout.FloatField(label, character.m_DetectionRadius);
            label = new GUIContent("Detection Height", "The height at which the agent can detect objects.");
            character.m_DetectionHeight = EditorGUILayout.FloatField(label, character.m_DetectionHeight);
            label = new GUIContent("Line of Sight", "The area where the agent will be able to see objects.");
            character.m_LineOfSight = EditorGUILayout.FloatField(label, character.m_LineOfSight);
            label = new GUIContent("Hit Points", "The total hit points of the character.");
            EditorGUILayout.TextField(label, character.HitPoints.ToString());
            label = new GUIContent("Is Player Controlled", "Whether or not this AI is controlled by the player.");
            character.isPlayerControlled = EditorGUILayout.Toggle(label, character.isPlayerControlled);
            if (character.isPlayerControlled)
            {
                label = new GUIContent("Camera", "The Main Camera GameObject.");
                character.cam = (Camera)EditorGUILayout.ObjectField(label, character.cam, typeof(Camera), true);
            }
            label = new GUIContent("Rig", "The rig that contains all the bones of the character.");
            character.rig = (GameObject)EditorGUILayout.ObjectField(label, character.rig, typeof(GameObject), true);
            label = new GUIContent("Enable ragdoll", "Enable ragdoll from the start.");
            character.enableRagdoll = EditorGUILayout.Toggle(label, character.enableRagdoll);
            label = new GUIContent("Enable Ragdoll on Death", "Enable ragdoll when the character dies.");
            character.enableRagdollOnDeath = EditorGUILayout.Toggle(label, character.enableRagdollOnDeath);

            

        }
        void DrawMovementGUI()
        {
            
            //Detection and Head Look Settings
            GUILayout.Label("General Movement Settings", guiStyle);
            GUIContent label;
            label = new GUIContent("Movement Speed", "The speed at which the agent will move at.");
            character.movementSpeed = EditorGUILayout.FloatField(label, character.movementSpeed);
            GUILayout.Space(16);
            label = new GUIContent("Movement Type", "Choose the movement type that this AI agent will use.");
            if(character.is3D)
                character.moveType = EditorGUILayout.Popup(label, character.moveType, arrMoveType);
            else
            {
                character.moveType = EditorGUILayout.Popup(label, character.moveType, arrMoveType2D);
            }
            if(character.moveType == MoveType.MOVE_PATHFIND)
            {
                label = new GUIContent("Pathfind Source", "Choose the pathfind source that this AI agent will use.");
                character.pathfindSource = EditorGUILayout.Popup(label, character.pathfindSource, arrPathfindSource);
                if(character.pathfindSource == AIController.PATHFIND_SOURCE_CANDICE)
                {
                    label = new GUIContent("Turn Speed", "");
                    character.turnSpeed = EditorGUILayout.FloatField(label, character.turnSpeed);
                    label = new GUIContent("Turn Distance", "");
                    character.turnDist = EditorGUILayout.FloatField(label, character.turnDist);
                    label = new GUIContent("Stopping Distance", "How far away the agent will start to come to a halt.");
                    character.stoppingDist = EditorGUILayout.FloatField(label, character.stoppingDist);
                }

                
            }
            GUILayout.Space(16);
            label = new GUIContent("Enable Head Look:", "Allow the agent to dynamically look at objects.");
            character.enableHeadLook = EditorGUILayout.Toggle(label, character.enableHeadLook);
            label = new GUIContent("Head Look target: ");
            character.headLookTarget = (GameObject)EditorGUILayout.ObjectField(label, character.headLookTarget, typeof(GameObject), true);
            label = new GUIContent("Head Look Intensity:", "How quickly the agent will turn their head to look at objects.");
            character.headLookIntensity = EditorGUILayout.Slider(label, character.headLookIntensity, 0f, 1f);

            //Patrol Settings
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            //GUIContent label = new GUIContent("Patrol Settings");

            //EditorGUILayout.LabelField();
            EditorGUILayout.LabelField("Patrol Settings", guiStyle);
            
            character.isPatrolling = EditorGUILayout.Toggle("Is Patrolling", character.isPatrolling);
            label = new GUIContent("Patrol In Order:", "Whether or not the character should patrol each point in order of the list. False will allow the character to patrol randomly.");
            character.patrolInOrder = EditorGUILayout.Toggle(label, character.patrolInOrder);
            label = new GUIContent("Patrol Points:", "The points in the gameworld where you want the character to patrol. They can be anything, even empty gameObjects. Note: Ensure each patrol point is tagged as 'PatrolPoint'");
            patrolPointCount = character.patrolPoints.Count;
            showPatrolPoints = EditorGUILayout.Foldout(showPatrolPoints, label);
            if(showPatrolPoints)
            {
                label = new GUIContent("Size:");
                patrolPointCount = EditorGUILayout.IntField(label, patrolPointCount);

                if (patrolPointCount != character.patrolPoints.Count)
                {
                    while (patrolPointCount > character.patrolPoints.Count)
                    {
                        character.patrolPoints.Add(null);
                    }
                    while (patrolPointCount < character.patrolPoints.Count)
                    {
                        character.patrolPoints.RemoveAt(character.patrolPoints.Count - 1);
                    }
                }
                //EditorGUILayout.Space();
                for (int i = 0; i < character.patrolPoints.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Element " + i);
                    character.patrolPoints[i] = (GameObject)EditorGUILayout.ObjectField(character.patrolPoints[i], typeof(GameObject), true);
                    EditorGUILayout.EndHorizontal();
                    //EditorGUILayout.Space();
                }
            }
        }
        void DrawCombatGUI()
        {
            GUILayout.Label("Attack Settings", guiStyle);
            GUIContent label = new GUIContent("Attack Type", "");
            character.attackType = EditorGUILayout.Popup(label, character.attackType, arrAttackTypes);
            if(character.attackType == AIController.ATTACK_TYPE_RANGE)
            {
                label = new GUIContent("Attack Range", "The range that the AI will start attacking enemies.");
                character.m_AttackRange = EditorGUILayout.FloatField(label, character.m_AttackRange);
                label = new GUIContent("Attack Projectile", "The projectile that the agent will fire.");
                character.attackProjectile = (GameObject)EditorGUILayout.ObjectField(label, character.attackProjectile, typeof(GameObject), true);

                label = new GUIContent("Projectile Spawn Position", "The point where the projectile will spawn..");
                character.spawnPosition = (Transform)EditorGUILayout.ObjectField(label, character.spawnPosition, typeof(Transform), true);
            }
            else
            {
                character.m_AttackRange = Character.m_DefaultAttackRange;
            }
            
            character.m_DamageAngle = EditorGUILayout.Slider("Damage Angle:", character.m_DamageAngle, 0, 360f);
            label = new GUIContent("Has Attack Animation", "Whether or not this agent has an attack animation.");
            character.hasAttackAnimation = EditorGUILayout.Toggle(label, character.hasAttackAnimation);
            label = new GUIContent("Attack Speed", "How many attacks per second the agent will deal");
            character.attacksPerSecond = EditorGUILayout.FloatField(label, character.attacksPerSecond);
            label = new GUIContent("Auto Attack", "");
            character.autoAttack = EditorGUILayout.Toggle(label, character.autoAttack);
            
            


            }
        void OnSceneGUI()
        {
            if(character != null)
            {
                //Call the necessary methods to draw the discs and handles on the editor
                if(character.is3D)
                {
                    Color color = new Color(0f, 0f, 0f, 0.15f);//Red
                    DrawDiscs(color, character.transform.position, Vector3.up, -character.transform.forward, ref character.m_DetectionRadius, "Detection Radius", float.MaxValue);
                    color = new Color(0f, 1f, 0f, 0.35f);//Green
                    DrawArcs(color, character.transform.position, Vector3.up, character.transform.forward, character.transform.forward, ref character.m_LineOfSight, ref character.m_DetectionRadius, "Line of Sight");
                    color = new Color(0f, 0f, 1f, 0.35f);//Blue
                    DrawDiscs(color, character.transform.position, Vector3.up, -character.transform.right, ref character.m_AttackRange, "Attack Range", character.m_DetectionRadius);
                    color = new Color(1f, 0f, 0f, 0.75f);//Red
                    DrawArcs(color, character.transform.position, Vector3.up, character.transform.forward, character.transform.right, ref character.m_DamageAngle, ref character.m_AttackRange, "Damage Angle");
                    
                }
                else
                {
                    Color color = new Color(1f, 0f, 0f, 0.15f);//Red
                    DrawDiscs(color, character.transform.position, Vector3.forward, character.transform.up, ref character.m_DetectionRadius, "Detection Radius", float.MaxValue);
                    color = new Color(0f, 0f, 1f, 0.35f);//Blue
                    DrawDiscs(color, character.transform.position, Vector3.forward, character.transform.right, ref character.m_AttackRange, "Attack Range", character.m_DetectionRadius);
                    //color = new Color(1f, 0f, 0f, 0.75f);//Red
                    //DrawArcs(color, character.transform.position, Vector3.forward, character.transform.up, character.transform.up, ref character.m_DamageAngle, ref character.m_AttackRange, "Damage Angle");
                    //color = new Color(0f, 1f, 0f, 0.35f);//Green
                    //DrawArcs(color, character.transform.position, Vector3.forward, character.transform.up, character.transform.forward, ref character.m_LineOfSight, ref character.m_DetectionRadius, "Line of Sight");
                }
                
            }
            
        }

        protected void DrawDiscs(Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius, float maxValue)
        {
            //
            //Method Name : void DrawDiscs(Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius)
            //Purpose     : This method draws the necessary discs and slider handles in the editor to adjust the attack range and detection radius.
            //Re-use      : none
            //Input       : Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius
            //Output      : none
            //
            //Draw the disc that will represent the detection radius
            Handles.color = color;
            Handles.DrawSolidDisc(center, normal, radius);
            Handles.color = new Color(1f, 1f, 0f, 0.75f);
            Handles.DrawWireDisc(center, normal, radius);

            //Create Slider handles to adjust detection radius properties
            color.a = 0.5f;
            Handles.color = color;
            radius = Handles.ScaleSlider(radius, character.transform.position, direction, Quaternion.identity, radius, 1f);
            radius = Mathf.Clamp(radius, 1f, maxValue);

            

        }
        
        protected void DrawDiscs(Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius, string label, float maxValue)
        {
            //
            //Method Name : void DrawDiscs(Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius, string label)
            //Purpose     : Overloaded method of DrawDiscs(Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius)
            //              that adds the necessary labels. 
            //Re-use      : DrawDiscs(Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius)
            //Input       : Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius, string label
            //Output      : none
            //

            DrawDiscs(color, center, normal, direction, ref radius, maxValue);
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontSize = 12;
            labelStyle.normal.textColor = new Color(0, 0, 0, 1);
            labelStyle.alignment = TextAnchor.UpperCenter;
            Handles.Label(character.transform.position + (direction * radius), label, labelStyle);
        }

        protected void DrawArcs(Color color, Vector3 center, Vector3 normal, Vector3 direction, Vector3 sliderDirection, ref float angle, ref float radius, string label)
        {
            //
            //Method Name : void DrawDiscs(Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius)
            //Purpose     : This method draws the necessary discs and slider handles in the editor to adjust the attack range and detection radius.
            //Re-use      : none
            //Input       : Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius
            //Output      : none
            //
            //Draw the disc that will represent the detection radius
            
            Handles.color = color;
            Vector3 newDirection = character.transform.forward - (character.transform.right);
            Handles.DrawSolidArc(center, normal, direction, angle/2, radius);
            Handles.DrawSolidArc(center, normal, direction, -angle/2, radius);
            Handles.color = new Color(1f, 1f, 0f, 0.75f);
            Handles.DrawWireArc(center, normal, newDirection, angle, radius);

            //Create Slider handles to adjust detection radius properties
            color.a = 0.5f;
            Handles.color = color;
            angle = Handles.ScaleSlider(angle, character.transform.position, sliderDirection, Quaternion.identity, radius, 1f);
            angle = Mathf.Clamp(angle, 1f, 360);

            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontSize = 12;
            labelStyle.normal.textColor = new Color(0,0,0,1);
            labelStyle.alignment = TextAnchor.UpperCenter;
            Handles.Label(character.transform.position + (sliderDirection * radius), label, labelStyle);
        }
        #endregion
    }//end class
}//end namespace

