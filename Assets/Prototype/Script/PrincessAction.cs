//using System.Collections.Generic;
//using UnityEngine;

//public class PrincessAction : MonoBehaviour
//{
//    public enum E_ACTIONSTATE
//    {
//        none,
//        move,

//        _count
//    }

//    private _FieldDataManager fdMng;
//    private CharacterMoveController cmController;
//    private E_ACTIONSTATE state;
//    private Vector3 targetPos;
//    private CommonSE_Proto.E_TIMEOFDAY timeOfDay;

//    private void Start()
//    {
//        cmController = GetComponent<CharacterMoveController>();
//        if (!cmController)
//        {
//            Debug.LogError(
//                "Script:PrincessAction.cs \n" +
//                "Princess ‚É cmController ‚ª‚Â‚¢‚Ä‚¢‚Ü‚¹‚ñ"
//                );
//            return;
//        }

//        fdMng = GameObject.Find("Field").GetComponentInChildren<_FieldDataManager>();
//        if (!fdMng)
//        {
//            Debug.LogError(
//               "Script:PrincessAction.cs \n" +
//               "_FieldDataManager‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ");
//        }

//    }

//    private void FixedUpdate()
//    {
//        Action(timeOfDay);
//    }

//    private void Action(CommonSE_Proto.E_TIMEOFDAY tod)
//    {
//        // TODO ‰¼
//        if (Input.GetKeyDown(KeyCode.I))
//        {
//            DecideTargetPos();
//            Debug.Log("targetPos.x : " + targetPos.x);
//            Debug.Log("targetPos.y : " + targetPos.y);
//        }

//        //switch(tod)
//        //{
//        //    case CommonSE_Proto.E_TIMEOFDAY.morning:

//        //        break;
//        //    case CommonSE_Proto.E_TIMEOFDAY.noon:
//        //        Noon();
//        //        break;
//        //    case CommonSE_Proto.E_TIMEOFDAY.night:

//        //        break;
//        //}
//    }

//    private void Noon()
//    {
//        //switch(state)
//        //{
//        //    case E_ACTIONSTATE.none:

//        //        DecideTargetPos();

//        //        break;
//        //    case E_ACTIONSTATE.move:
                
//        //        break;
//        //}
//    }


//    private void DecideTargetPos()
//    {
//        FieldDataManager.S_FIELDINFO princessInfo = new FieldDataManager.S_FIELDINFO();
//        FieldDataManager.S_FIELDINFO goalInfo = new FieldDataManager.S_FIELDINFO();
//        List<FieldDataManager.S_FIELDINFO> obstacleInfoList = new List<FieldDataManager.S_FIELDINFO>();

//        Vector2Int listLen = fdMng.GetFieldSize();

//        for (int y = 0; y < listLen.y; ++y)
//        {
//            for (int x = 0; x < listLen.x; ++x)
//            {
//                switch(fdMng.fieldInfoArray[x, y].state)
//                {
//                    case FieldDataManager.E_FIELDSTATE.none:
//                        break;
//                    case FieldDataManager.E_FIELDSTATE.princess:
//                        princessInfo = fdMng.fieldInfoArray[x, y];
//                        break;
//                    case FieldDataManager.E_FIELDSTATE.goal:
//                        goalInfo = fdMng.fieldInfoArray[x, y];
//                        break;
//                    case FieldDataManager.E_FIELDSTATE.obstacle:
//                        obstacleInfoList.Add(fdMng.fieldInfoArray[x, y]);
//                        break;
//                    default:
//                        break;
//                }
//            }
//        }

//        List<Vector2> candidatePositions = new List<Vector2>();
//        float minDistanceToPrincess = float.MaxValue;

//        // áŠQ•¨‚ÌŽüˆÍ4ƒ}ƒX‚ðŒó•â‚Æ‚µ‚Ä’Šo
//        foreach (var obs in obstacleInfoList)
//        {
//            Vector2[] around = new Vector2[]
//            {
//            obs.pos + Vector2.up,
//            obs.pos + Vector2.down,
//            obs.pos + Vector2.left,
//            obs.pos + Vector2.right
//            };

//            foreach (var pos in around)
//            {
//                int x = (int)pos.x;
//                int y = (int)pos.y;

//                if (x >= 0 && x < arrayLenX && y >= 0 && y < arrayLenY)
//                {
//                    var cell = fdMng.fieldInfoArray[x, y];
//                    if (cell.state == FieldDataManager.E_FIELDSTATE.none)
//                    {
//                        float distToPrincess = Vector2.Distance(princessInfo.pos, cell.pos);
//                        if (distToPrincess < minDistanceToPrincess)
//                        {
//                            candidatePositions.Clear();
//                            candidatePositions.Add(cell.pos);
//                            minDistanceToPrincess = distToPrincess;
//                        }
//                        else if (Mathf.Approximately(distToPrincess, minDistanceToPrincess))
//                        {
//                            candidatePositions.Add(cell.pos);
//                        }
//                    }
//                }
//            }
//        }

//        // ƒS[ƒ‹‚Æ‚Ì‹——£‚Å‚³‚ç‚ÉŒó•â‚ði‚é
//        float minDistanceToGoal = float.MaxValue;
//        List<Vector2> bestCandidates = new List<Vector2>();
//        foreach (var pos in candidatePositions)
//        {
//            float distToGoal = Vector2.Distance(goalInfo.pos, pos);
//            if (distToGoal < minDistanceToGoal)
//            {
//                bestCandidates.Clear();
//                bestCandidates.Add(pos);
//                minDistanceToGoal = distToGoal;
//            }
//            else if (Mathf.Approximately(distToGoal, minDistanceToGoal))
//            {
//                bestCandidates.Add(pos);
//            }
//        }

//        // ŽžŒv‰ñ‚è—Dæ‚Å1‚Â‘I‚Ô
//        Vector2[] directions = GetClockwiseDirections(princessInfo.dir);
//        Vector2 finalTarget = bestCandidates[0];
//        float minDirDist = float.MaxValue;

//        foreach (var dir in directions)
//        {
//            Vector2 checkPos = princessInfo.pos + dir;
//            foreach (var pos in bestCandidates)
//            {
//                if (pos == checkPos)
//                {
//                    finalTarget = pos;
//                    goto FoundFinalTarget;
//                }

//                float d = Vector2.Distance(checkPos, pos);
//                if (d < minDirDist)
//                {
//                    minDirDist = d;
//                    finalTarget = pos;
//                }
//            }
//        }

//    FoundFinalTarget:
//        targetPos = finalTarget;

//        cmController.AddPosX((int)finalTarget.x);
//        cmController.AddPosY((int)finalTarget.y);
        

//    }

//    private void Move()
//    {
        
//    }


//    private Vector2[] GetClockwiseDirections(CommonSE_Proto.E_DIRECTION forward)
//    {
//        switch (forward)
//        {
//            case CommonSE_Proto.E_DIRECTION.up:
//                return new Vector2[] { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
//            case CommonSE_Proto.E_DIRECTION.right:
//                return new Vector2[] { Vector2.right, Vector2.down, Vector2.left, Vector2.up };
//            case CommonSE_Proto.E_DIRECTION.down:
//                return new Vector2[] { Vector2.down, Vector2.left, Vector2.up, Vector2.right };
//            case CommonSE_Proto.E_DIRECTION.left:
//                return new Vector2[] { Vector2.left, Vector2.up, Vector2.right, Vector2.down };
//            default:
//                return new Vector2[] { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
//        }
//    }
//}
