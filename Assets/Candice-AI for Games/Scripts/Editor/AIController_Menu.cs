using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using UnityEngine.AI;

namespace ViridaxGameStudios.AI
{
    public class AIController_Menu : EditorWindow
    {
        private GameObject obj;
        private bool attachRigidBody = false;
        private bool attachCollider = false;
        private bool attachAnimator = false;
        private int colliderTypeIndex = 0;
        private int colliderTypeIndex2D = 0;
        private string[] arrColliderTypes = {"Capsule","Box","Sphere","Mesh" };
        private string[] arrColliderTypes2D = { "Capsule", "Box", "Circle" };
        string[] arrAttackTypes = { "Melee", "Range" };
        private int attackType;
        float m_AttackRange = 7f;
        private bool attachNavAgent = false;
        bool is3D;
        bool isEnemy;
        private GameObject attackProjectile;
        private Transform spawnPosition;

        private GameObject rig;
        private bool enableRagdoll = false;
        private bool ragdollOnDeath = true;
        bool hasAttackAnimation = true;
        float movementSpeed = 7f;
        Rect headerRect;
        Rect mainRect;
        GUIStyle guiStyle;
        [MenuItem("Window/Viridax Game Studios/AI/" + CandiceConfig.CONTROLLER_NAME)]
        public static void AddController()
        {
            GameObject[] selectedGO = Selection.gameObjects;
            if (selectedGO.Length > 0)
            {
                foreach (GameObject obj in selectedGO)
                {
                    ShowWindow(obj);
                }
            }
            else
            {
                EditorUtility.DisplayDialog(CandiceConfig.APP_NAME, "You need to select at least 1 GameObject", "OK");
            }

            
        }
    
        static void ShowWindow(GameObject obj)
        {
            EditorWindow window = GetWindow<AIController_Menu>();
            (window as AIController_Menu).Initialise(obj);
            window.titleContent = new GUIContent(CandiceConfig.CONTROLLER_NAME + ": Setup");
            window.Show();
        }


        public void Initialise(GameObject _obj)
        {
            obj = _obj;
        }

        void AttachAIControllerScript(GameObject obj)
        {


            //Assign AI Script to the GameObject
            AIController AIscript = null;
            if (obj)
            {
                //Check if the object has a rigidbody
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    obj.AddComponent<Rigidbody>();
                }

                //Check if the object has a Collider
                Collider col = obj.GetComponent<Collider>();
                if (col == null)
                {
                    obj.AddComponent<CapsuleCollider>();
                }

                AIscript = obj.AddComponent<AIController>();
                AIscript.enemyTags.Add("Player");
                Selection.activeGameObject = obj;
            }
        }

        private void OnGUI()
        {
            float width = Screen.width / 2 + Screen.width / 4 + Screen.width / 32;
            headerRect = new Rect(0, 0, width, 50f);
            mainRect = new Rect(0, headerRect.yMax, width, 500f);
            guiStyle = new GUIStyle();
            guiStyle.fontSize = 28;
            guiStyle.fontStyle = FontStyle.Bold;

            GUILayout.BeginVertical();//1
            
            GUILayout.Space(4);
            GUILayout.BeginArea(headerRect);
            GUILayout.FlexibleSpace();
            GUILayout.Label(" " + CandiceConfig.CONTROLLER_NAME + " Setup Assistant", guiStyle);
            GUILayout.FlexibleSpace();
            is3D = EditorGUILayout.Toggle("Is 3D", is3D);
            GUILayout.EndArea();
            GUILayout.Space(8);
            GUILayout.BeginArea(mainRect);
            if (is3D)
                Setup3D();
            else
                Setup2D();

            
            
            GUILayout.EndArea();
        }

        void Setup2D()
        {
            

            //Check if the object has a rigidbody
            GUILayout.BeginVertical("box");
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                GUILayout.Label("No Rigidbody detected. One will be automatically added");
                attachRigidBody = true;
            }
            else
            {
                GUILayout.Label("Rigidbody detected.");
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            //Check if the object has a Collider
            Collider2D col = obj.GetComponent<Collider2D>();
            if (col == null)
            {
                GUILayout.Label("No Collider detected. One will be automatically added");
                colliderTypeIndex2D = EditorGUILayout.Popup("Collider Type", colliderTypeIndex2D, arrColliderTypes2D);
                attachCollider = true;
            }
            else
            {
                EditorGUILayout.LabelField("Collider detected.");
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            //Check if the object has a Animator
            Animator anim = obj.GetComponent<Animator>();
            if (anim == null)
            {
                GUILayout.Label("No Animator detected. One will be automatically added");
                attachAnimator = true;
            }
            else
            {
                EditorGUILayout.LabelField("Animator detected.");
            }
            GUILayout.EndVertical();
            GUILayout.Space(16);
            GUIContent label = new GUIContent();
            label = new GUIContent("Movement Speed", "The speed at which the agent will move at.");
            movementSpeed = EditorGUILayout.FloatField(label, movementSpeed);
            GUILayout.Space(8);


            label = new GUIContent("Rig", "The rig that contains all the bones of the character.");
            rig = (GameObject)EditorGUILayout.ObjectField(label, rig, typeof(GameObject), true);
            label = new GUIContent("Enable ragdoll", "Enable ragdoll from the start.");
            enableRagdoll = EditorGUILayout.Toggle(label, enableRagdoll);
            label = new GUIContent("Enable Ragdoll on Death", "Enable ragdoll when the character dies.");
            ragdollOnDeath = EditorGUILayout.Toggle(label, ragdollOnDeath);
            GUILayout.Space(8);

            label = new GUIContent("Attack Type", "");
            attackType = EditorGUILayout.Popup(label, attackType, arrAttackTypes);
            if (attackType == AIController.ATTACK_TYPE_RANGE)
            {
                label = new GUIContent("Attack Range", "The range that the AI will start attacking enemies.");
                m_AttackRange = EditorGUILayout.FloatField(label, m_AttackRange);
                label = new GUIContent("Attack Projectile", "The projectile that the agent will fire.");
                attackProjectile = (GameObject)EditorGUILayout.ObjectField(label, attackProjectile, typeof(GameObject), true);

                label = new GUIContent("Projectile Spawn Position", "The point where the projectile will spawn..");
                spawnPosition = (Transform)EditorGUILayout.ObjectField(label, spawnPosition, typeof(Transform), true);
            }
            else
            {
                m_AttackRange = Character.m_DefaultAttackRange;
            }
            label = new GUIContent("Has Attack Animation", "Whether or not this agent has an attack animation.");
            hasAttackAnimation = EditorGUILayout.Toggle(label, hasAttackAnimation);
            label = new GUIContent("Is Enemy", "Whether or not this agent is an enemy of the player.");
            isEnemy = EditorGUILayout.Toggle(label, isEnemy);
            guiStyle.fontSize = 10;
            GUILayout.Space(4);
            GUILayout.Label("*Note: You can still edit these, and other settings on the AI Controller component after it is created.", guiStyle);
            GUILayout.Space(4);
            if (GUILayout.Button("Add Controller"))
            {
                if (attachRigidBody)
                    obj.AddComponent<Rigidbody2D>();
                if (attachNavAgent)
                    obj.AddComponent<NavMeshAgent>();
                if (attachAnimator)
                {
                    Animator a = obj.AddComponent<Animator>();
                    a.applyRootMotion = false;
                }

                if (attachCollider)
                {
                    switch (colliderTypeIndex2D)
                    {
                        case 0:
                            obj.AddComponent<CapsuleCollider2D>();
                            break;
                        case 1:
                            obj.AddComponent<BoxCollider2D>();
                            break;
                        case 2:
                            obj.AddComponent<CircleCollider2D>();
                            break;
                    }
                }
                AIController aiScript = null;
                aiScript = obj.AddComponent<AIController>();
                aiScript.is3D = is3D;
                aiScript.movementSpeed = movementSpeed;
                if(isEnemy)
                    aiScript.enemyTags.Add("Player");
                aiScript.rig = rig;
                aiScript.enableRagdoll = enableRagdoll;
                aiScript.enableRagdollOnDeath = ragdollOnDeath;

                aiScript.attackType = attackType;
                aiScript.m_AttackRange = m_AttackRange;
                aiScript.attackProjectile = attackProjectile;
                aiScript.spawnPosition = spawnPosition;
                aiScript.hasAttackAnimation = hasAttackAnimation;
                Selection.activeGameObject = obj;
                EditorUtility.DisplayDialog(CandiceConfig.APP_NAME, "Controller Setup Complete", "OK");
                Close();
            }
            GUILayout.EndVertical();//1
        }


        void Setup3D()
        {
            
            //Check if the object has a rigidbody
            GUILayout.BeginVertical("box");
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null)
            {
                GUILayout.Label("No Rigidbody detected. One will be automatically added");
                attachRigidBody = true;
            }
            else
            {
                GUILayout.Label("Rigidbody detected.");
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            //Check if the object has a Collider
            Collider col = obj.GetComponent<Collider>();
            if (col == null)
            {
                GUILayout.Label("No Collider detected. One will be automatically added");
                colliderTypeIndex = EditorGUILayout.Popup("Collider Type", colliderTypeIndex, arrColliderTypes);
                attachCollider = true;
            }
            else
            {
                EditorGUILayout.LabelField("Collider detected.");
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            //Check if the object has a Animator
            Animator anim = obj.GetComponent<Animator>();
            if (anim == null)
            {
                GUILayout.Label("No Animator detected. One will be automatically added");
                attachAnimator = true;
            }
            else
            {
                EditorGUILayout.LabelField("Animator detected.");
            }
            GUILayout.EndVertical();
            GUILayout.Space(16);
            GUIContent label = new GUIContent();
            label = new GUIContent("Movement Speed", "The speed at which the agent will move at.");
            movementSpeed = EditorGUILayout.FloatField(label, movementSpeed);
            GUILayout.Space(8);


            label = new GUIContent("Rig", "The rig that contains all the bones of the character.");
            rig = (GameObject)EditorGUILayout.ObjectField(label, rig, typeof(GameObject), true);
            label = new GUIContent("Enable ragdoll", "Enable ragdoll from the start.");
            enableRagdoll = EditorGUILayout.Toggle(label, enableRagdoll);
            label = new GUIContent("Enable Ragdoll on Death", "Enable ragdoll when the character dies.");
            ragdollOnDeath = EditorGUILayout.Toggle(label, ragdollOnDeath);
            GUILayout.Space(8);

            label = new GUIContent("Attack Type", "");
            attackType = EditorGUILayout.Popup(label, attackType, arrAttackTypes);
            if (attackType == AIController.ATTACK_TYPE_RANGE)
            {
                label = new GUIContent("Attack Range", "The range that the AI will start attacking enemies.");
                m_AttackRange = EditorGUILayout.FloatField(label, m_AttackRange);
                label = new GUIContent("Attack Projectile", "The projectile that the agent will fire.");
                attackProjectile = (GameObject)EditorGUILayout.ObjectField(label, attackProjectile, typeof(GameObject), true);

                label = new GUIContent("Projectile Spawn Position", "The point where the projectile will spawn..");
                spawnPosition = (Transform)EditorGUILayout.ObjectField(label, spawnPosition, typeof(Transform), true);
            }
            else
            {
                m_AttackRange = Character.m_DefaultAttackRange;
            }
            label = new GUIContent("Has Attack Animation", "Whether or not this agent has an attack animation.");
            hasAttackAnimation = EditorGUILayout.Toggle(label, hasAttackAnimation);
            label = new GUIContent("Is Enemy", "Whether or not this agent is an enemy of the player.");
            isEnemy = EditorGUILayout.Toggle(label, isEnemy);
            guiStyle.fontSize = 10;
            GUILayout.Space(4);
            GUILayout.Label("*Note: You can still edit these, and other settings on the AI Controller component after it is created.", guiStyle);
            GUILayout.Space(4);
            if (GUILayout.Button("Add Controller"))
            {
                if (attachRigidBody)
                    obj.AddComponent<Rigidbody>();
                if (attachNavAgent)
                    obj.AddComponent<NavMeshAgent>();
                if (attachAnimator)
                {
                    Animator a = obj.AddComponent<Animator>();
                    a.applyRootMotion = false;
                }

                if (attachCollider)
                {
                    switch (colliderTypeIndex)
                    {
                        case 0:
                            obj.AddComponent<CapsuleCollider>();
                            break;
                        case 1:
                            obj.AddComponent<BoxCollider>();
                            break;
                        case 2:
                            obj.AddComponent<SphereCollider>();
                            break;
                        case 3:
                            obj.AddComponent<MeshCollider>();
                            break;
                    }
                }
                AIController aiScript = null;
                aiScript = obj.AddComponent<AIController>();
                aiScript.is3D = is3D;
                aiScript.movementSpeed = movementSpeed;
                if(isEnemy)
                    aiScript.enemyTags.Add("Player");
                aiScript.rig = rig;
                aiScript.enableRagdoll = enableRagdoll;
                aiScript.enableRagdollOnDeath = ragdollOnDeath;

                aiScript.attackType = attackType;
                aiScript.m_AttackRange = m_AttackRange;
                aiScript.attackProjectile = attackProjectile;
                aiScript.spawnPosition = spawnPosition;
                aiScript.hasAttackAnimation = hasAttackAnimation;
                Selection.activeGameObject = obj;
                EditorUtility.DisplayDialog(CandiceConfig.APP_NAME, "Controller Setup Complete", "OK");
                Close();
            }
            GUILayout.EndVertical();//1
        }
    }
}