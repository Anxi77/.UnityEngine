using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SpaceShooter
{
    public class PlayerScript : MonoBehaviour
    {
        public float moveSpeed = 5f;
        #region Backup
        //public float boundaryXMinSize = -15.22f;
        //public float boundaryXMaxSize = 15.22f;
        //public float boundaryYMinSize = -18.62f;
        //public float boundaryYMaxSize = 18.62f;
        //public Renderer target;
         // 회전 속도 조절을 위한 변수 추가
        #endregion
        public GameObject gameOverMessage;
        private Rigidbody rb;
        private float zOffset;
        public float rotationSpeed = 5f;
        public float resetSpeed = 2f;
        private float initialZRotation;


        private void Awake()
        {
            //target = GetComponentInChildren<Renderer>();
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            PlayerMove();
            PlayerRotate();
        }

        private void Start()
        {
            initialZRotation = transform.rotation.eulerAngles.z;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                //print("Game Over");
                //GameOver();
                Destroy(collision.gameObject);
            }
        }

        public void GameOver()
        {   
            gameOverMessage.SetActive(true);
            Time.timeScale = 0;
        }

        public void PlayerMove()
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            if (x != 0 || y != 0)
            {
                rb.MovePosition(transform.position + (new Vector3(x, y, 0) * Time.deltaTime * moveSpeed));
            }
            #region 2D to 3d
            //Vector3 movement = new Vector3(x, y, 0).normalized;
            //Vector3 newPosition = transform.position + movement * Time.deltaTime * moveSpeed;
            //rb.MovePosition(newPosition);

            //if (movement != Vector3.zero)
            //{
            //    float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            //    Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            //    rb.MoveRotation(rotation);
            //}
            //rb.AddForce(transform.forward * accelerationForce);
            #endregion
            #region nonRigidBody
            // 이동 처리


            //Vector3 movement = new Vector3(x, y, 0);
            //
            //Vector3 targetPosition = transform.position + movement * moveSpeed * Time.deltaTime;
            //transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            //transform.position = new Vector3(
            //    Mathf.Round(transform.position.x * 100f) / 100f,
            //    Mathf.Round(transform.position.y * 100f) / 100f,
            //    transform.position.z
            //);

            //// X축 경계 처리
            //transform.position = new Vector3(
            //    Mathf.Clamp(transform.position.x, boundaryXMinSize, boundaryXMaxSize),
            //    transform.position.y,
            //    transform.position.z
            //);

            //// Y축 경계 처리
            //transform.position = new Vector3(
            //    transform.position.x,
            //    Mathf.Clamp(transform.position.y, boundaryYMinSize, boundaryYMaxSize),
            //    transform.position.z
            //);
            #endregion
        }

        public void PlayerRotate()
        {

            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            if (x != 0 || y != 0)
            {
                float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation * Quaternion.Euler(-90f, 0f, angle + 90f), 5f * Time.deltaTime));
            }

            #region Old
            //if (Mathf.Abs(x) > 0.1f) // 좌우 움직임이 있을 때만
            //{
            //    float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            //    Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);
            //    rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation * Quaternion.Euler(-90f, 0f, angle + 90f), rotationSpeed * Time.fixedDeltaTime));
            //}
            //else // 좌우 움직임이 없을 때
            //{
            //    // 현재 x, y 회전을 유지하고 z 회전만 초기화
            //    Vector3 currentRotation = transform.rotation.eulerAngles;
            //    Quaternion targetRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, initialZRotation);
            //    rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
            //}
            //if (Mathf.Abs(x) > 0.1f) // 좌우 움직임이 있을 때만
            //{
            //    float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            //    Quaternion currentRotation = transform.rotation;
            //    Vector3 currentEuler = currentRotation.eulerAngles;

            //    // x와 y 회전은 유지하고 z 회전만 변경
            //    Quaternion targetRotation = Quaternion.Euler(currentEuler.x, currentEuler.y, angle - 90f);

            //    rb.MoveRotation(Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
            //}
            //else // 좌우 움직임이 없을 때
            //{
            //    // 현재 x, y 회전은 유지하고 z 회전만 초기값으로 즉시 되돌림
            //    Vector3 currentEuler = transform.rotation.eulerAngles;
            //    Quaternion targetRotation = Quaternion.Euler(currentEuler.x, currentEuler.y, initialZRotation);
            //    rb.MoveRotation(targetRotation);
            //}

            //if (movement == Vector3.zero)
            //{
            //    Vector3 currentEuler = transform.rotation.eulerAngles;
            //    float newZRotation = Mathf.LerpAngle(currentEuler.z, initialLocalRotation.eulerAngles.z, resetSpeed * Time.deltaTime);
            //    Quaternion targetRotation = Quaternion.Euler(currentEuler.x, currentEuler.y, newZRotation);
            //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2000f * Time.deltaTime);
            //}


            //float x = Input.GetAxis("Horizontal");
            //float y = Input.GetAxis("Vertical");

            //Vector3 movement = new Vector3(x, y, 0).normalized;

            //if (movement != Vector3.zero)
            //{
            //    // 이동 방향에 따른 z축 회전 계산
            //    float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

            //    // 현재 로컬 회전에서 z 회전만 변경
            //    Quaternion deltaRotation = Quaternion.Euler(0, 0, angle - 90f);
            //    Quaternion targetRotation = transform.localRotation * deltaRotation;

            //    // 현재 회전에서 목표 회전으로 부드럽게 전환
            //    transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
            //}
            //else
            //{
            //    // 움직임이 없을 때 z축 회전 초기화
            //    Quaternion targetRotation = Quaternion.Euler(
            //        transform.localRotation.eulerAngles.x,
            //        transform.localRotation.eulerAngles.y,
            //        initialLocalRotation.eulerAngles.z
            //    );
            //    transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, resetSpeed * Time.deltaTime);
            //}
            #endregion
            #region NonRigidBody
            //float x = Input.GetAxis("Horizontal");
            //float y = Input.GetAxis("Vertical");

            //if (x != 0 || y != 0)
            //{

            //    float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

            //    Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);

            //    transform.rotation = Quaternion.Slerp(transform.rotation , targetRotation * Quaternion.Euler(-90f, 0f, 0f), rotationSpeed * Time.deltaTime);

            //    if(x != 0) 
            //    {
            //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, 0f, 180f) , 5f * Time.deltaTime);
            //    }
            //}
            #endregion
        }
    }
}
