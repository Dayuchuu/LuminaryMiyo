using System;
using UnityEngine;
using UnityEngine.Rendering;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;

// [ExecuteInEditMode]
public class Portal : MonoBehaviour
{
    [SerializeField] private Camera portalCamera;
    [SerializeField] private Transform pairPortal;

    [SerializeField] private GameObject player;
    [SerializeField] private BoxCollider portalCollider;

    [SerializeField] private LayerMask groundLayerMask;
    private float distanceEnter;
    private float distanceExit;

    [SerializeField] private AudioSource portalSound;

    private void OnEnable()
    {
        RenderPipeline.beginCameraRendering += UpdateCamera;
    }

    private void OnDisable()
    {
        RenderPipeline.beginCameraRendering -= UpdateCamera;
    }


    private void UpdateCamera(ScriptableRenderContext context, Camera camera)
    {
        if ((camera.cameraType == CameraType.Game || camera.cameraType == CameraType.SceneView) &&
                camera.tag != "Portal Camera")
        {
            portalCamera.projectionMatrix = camera.projectionMatrix;

            var relativePosition = transform.InverseTransformPoint(camera.transform.position);
            relativePosition = Vector3.Scale(relativePosition, new Vector3(-1, 1, -1));
            portalCamera.transform.position = pairPortal.TransformPoint(relativePosition);

            var relativeRotation = transform.InverseTransformDirection(camera.transform.forward);
            relativeRotation = Vector3.Scale(relativeRotation, new Vector3(-1, 1, -1));
            portalCamera.transform.forward = pairPortal.TransformDirection(relativeRotation);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            Debug.Log("Entered Collider");
            
            RaycastHit hit;
            if (Physics.Raycast(player.transform.position, Vector3.down, out hit, 30f))
            {
                Debug.Log("Hit Something");
                Debug.Log(hit.distance);
                distanceEnter = hit.distance;

                //player.transform.position = new Vector3(pairPortal.transform.position.x, distanceEnter + player.transform.position.y/2, pairPortal.transform.position.z);
                player.transform.position = pairPortal.transform.position;
            }

        }
    }
}


//check height before teleport
//shoot raycast down 
//save length of ray
//teleport to new location
//shoot raycast down
//check length of ray
