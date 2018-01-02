using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    string TAG = "[CameraController]";

    float mMoveSpeed = 1f;

    float mRotateSpeed = 2f;

    private Vector3 mOriginalPosition = Vector3.zero;

    Vector3 mHitInfoPoint = Vector3.zero;
    Vector3 mRotateAroundPoint = Vector3.zero;

    void Start() {

    }

    void Update() {

    }

    void LateUpdate() {
        //if (Input.GetKey(KeyCode.W)) {
        //    //this.transform.Translate(Vector3.forward * mMoveSpeed, Space.World);
        //    Vector3 direction1 = transform.TransformDirection(Vector3.forward);
        //    this.transform.Translate(new Vector3(direction1.x, 0, direction1.z) * mMoveSpeed, Space.World);
        //}
        //if (Input.GetKey(KeyCode.S)) {
        //    //this.transform.Translate(Vector3.back * mMoveSpeed, Space.World);
        //    Vector3 direction1 = transform.TransformDirection(Vector3.forward);
        //    this.transform.Translate(new Vector3(direction1.x, 0, direction1.z) * -mMoveSpeed, Space.World);
        //}
        //if (Input.GetKey(KeyCode.A)) {
        //    this.transform.Translate(Vector3.left * mMoveSpeed, Space.Self);
        //}
        //if (Input.GetKey(KeyCode.D)) {
        //    this.transform.Translate(Vector3.right * mMoveSpeed, Space.Self);
        //}

        //if (Input.GetMouseButton(1)) {
        //    //mOriginalPosition = Input.mousePosition;
        //    Ray ray = new Ray(Camera.main.transform.position, transform.TransformDirection(Vector3.forward));
        //    RaycastHit hitInfo;
        //    if (Physics.Raycast(ray, out hitInfo)) {
        //        GameObject gameObj = hitInfo.collider.gameObject;
        //        mHitInfoPoint = hitInfo.point;
        //        mRotateAroundPoint = new Vector3(mHitInfoPoint.x, transform.position.y, mHitInfoPoint.z);
        //    }

        //    //float tempX = Input.mousePosition.x - mOriginalPosition.x;
        //    //if (tempX > 0) {
        //    //    transform.RotateAround(mRotateAroundPoint, Vector3.up, mRotateSpeed);
        //    //    transform.LookAt(mHitInfoPoint);
        //    //} else if (tempX < 0) {
        //    //    transform.RotateAround(mRotateAroundPoint, Vector3.up, -mRotateSpeed);
        //    //    transform.LookAt(mHitInfoPoint);
        //    //}

        //    float tempY = Input.mousePosition.y - mOriginalPosition.y;
        //    if (tempY > 0 && transform.position.y < 110) {
        //        transform.Translate(Vector3.up * mMoveSpeed, Space.World);
        //        transform.LookAt(mHitInfoPoint);
        //    } else if (tempY < 0 && transform.position.y > 10) {
        //        transform.Translate(Vector3.down * mMoveSpeed, Space.World);
        //        transform.LookAt(mHitInfoPoint);
        //    }

        //    mOriginalPosition = Input.mousePosition;
        //}

        //if (Input.GetMouseButtonUp(1)) {
        //    mOriginalPosition = Vector3.zero;
        //    mRotateAroundPoint = Vector3.zero;
        //}
        //Vector3 direction = transform.TransformDirection(Vector3.forward) * 50;
        //Debug.DrawRay(transform.position, direction, Color.green);

        ////Zoom out
        //if (Input.GetAxis("Mouse ScrollWheel") < 0) {
        //    if (Camera.main.fieldOfView < 45)
        //        Camera.main.fieldOfView += 2;
        //    if (Camera.main.orthographicSize <= 20)
        //        Camera.main.orthographicSize += 0.5F;
        //}
        ////Zoom in
        //if (Input.GetAxis("Mouse ScrollWheel") > 0) {
        //    if (Camera.main.fieldOfView > 20)
        //        Camera.main.fieldOfView -= 2;
        //    if (Camera.main.orthographicSize >= 1)
        //        Camera.main.orthographicSize -= 0.5F;
        //}
        if (Input.GetMouseButtonDown(0)) {
            mOriginalPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(0)) {
            Vector3 temp = Input.mousePosition - mOriginalPosition;
            temp = temp * 0.05f;
            transform.position = new Vector3(transform.position.x - temp.x , transform.position.y, transform.position.z - temp.y);
            mOriginalPosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0)) {
        }

    }



    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * 50;
        Gizmos.DrawRay(transform.position, direction);
    }
}
