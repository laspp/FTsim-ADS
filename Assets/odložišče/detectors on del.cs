// //SKRIPTA ZA PREVERJANJA PREKRIVANJA DETEKTORJA KLICANA OB IZBRISU KOSA
// using UnityEngine;

// public class PanelControls88 : MonoBehaviour
// {
//     public GameObject workpiece;
//     public Transform spawnPoint;
//     public GameObject detector;

//     void Update()
//     {
//         CheckOverlap();
//     }

//     public void CreateNewWorkpiece()
//     {
//         Vector3 pos = spawnPoint.position;
//         GameObject clone = Instantiate(workpiece, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity);
//         clone.tag = "Workpiece";
//         clone.SetActive(true);
//     }

//     public void RemoveWorkpiece()
//     {
//         GameObject[] workPieces = GameObject.FindGameObjectsWithTag("Workpiece");
//         if (workPieces != null && workPieces.Length > 0)
//         {
//             GameObject workPieceToRemove = workPieces[0];
//             Destroy(workPieceToRemove);

//             // Find the WorkpieceCollision component on the detector
//             WorkpieceCollision workpieceCollision = detector.GetComponent<WorkpieceCollision>();
//             if (workpieceCollision != null)
//             {
//                 // Create a temporary BoxCollider to simulate the OnTriggerExit call
//                 BoxCollider tempCollider = workPieceToRemove.AddComponent<BoxCollider>();
//                 workpieceCollision.OnTriggerExit(tempCollider);
//                 Destroy(tempCollider); // Clean up the temporary collider
//             }
//             else
//             {
//                 Debug.LogError("WorkpieceCollision component not found on detector.");
//             }
//         }
//     }

//     private void CheckOverlap()
//     {
//         GameObject[] workPieces = GameObject.FindGameObjectsWithTag("Workpiece");
//         if (workPieces != null && workPieces.Length > 0)
//         {
//             Collider detectorCollider = detector.GetComponent<Collider>();
//             foreach (GameObject workPiece in workPieces)
//             {
//                 Collider workPieceCollider = workPiece.GetComponent<Collider>();
//                 if (workPieceCollider != null && detectorCollider != null)
//                 {
//                     if (detectorCollider.bounds.Intersects(workPieceCollider.bounds))
//                     {
//                         Debug.Log("Workpiece is inside the detector.");
//                         // Perform any additional logic here
//                     }
//                 }
//             }
//         }
//     }

//     public void ResetScene()
//     {
//         // Your reset scene logic here
//     }
// }