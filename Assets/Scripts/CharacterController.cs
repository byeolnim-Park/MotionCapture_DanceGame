using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterController : MonoBehaviour
{
    public class currentJoint
    {
        public Transform Transform = null;
        public Vector3 InitPos;
        public Quaternion InitRot;
        public Quaternion InitInverse;
        public Quaternion InverseRotation;
        public currentJoint Child;
        public Vector3 Pos3D;
        public int LMnum;
    }

    private Vector3 initPosition; //초기 위치
    private Vector3 prevPosition; //이전 위치
    [Range(1, 10)]
    public int pos_scale;
    Vector3 forward;

    private enum JointIndex : int
    {
        Hip = 0,

        LegUp_L,
        LegDown_L,
        Foot_L,
        Toe_L,

        LegUp_R,
        LegDown_R,
        Foot_R,
        Toe_R,

        ArmUp_L,
        ArmDown_L,
        Hand_L,
        Mid_L,
        Thumb_L,

        ArmUp_R,
        ArmDown_R,
        Hand_R,
        Mid_R,
        Thumb_R,

        Neck,
        Head,
        Nose,
        Eye_L,
        Eye_R
    }


    private enum landmarkIndex : int
    {
        nose = 0,
        left_eye_inner,
        left_eye,
        left_eye_outer,
        right_eye_inner,
        right_eye,
        right_eye_outer,
        left_ear,
        right_ear,
        mouth_left,
        mouth_right,
        left_shoulder,
        right_shoulder,
        left_elbow,
        right_elbow,
        left_wrist,
        right_wrist,
        left_pinky,
        right_pinky,
        left_index,
        right_index,
        left_thumb,
        right_thumb,
        left_hip,
        right_hip,
        left_knee,
        right_knee,
        left_ankle,
        right_ankle,
        left_heel,
        right_heel,
        left_foot_index,
        right_foot_index
    }

    private Animator anim;
    private currentJoint[] Joints;
    //public currentJoint[] joints { get { return Joints; } }

    string[] Names = new string[33] { "nose", "left_eye_inner", "left_eye", "left_eye_outer", "right_eye_inner", "right_eye",
        "right_eye_outer", "left_ear", "right_ear", "mouth_left", "mouth_right", "left_shoulder", "right_shoulder",
        "left_elbow", "right_elbow", "left_wrist", "right_wrist", "left_pinky", "right_pinky", "left_index", "right_index",
        "left_thumb", "right_thumb", "left_hip", "right_hip", "left_knee", "right_knee", "left_ankle", "right_ankle",
        "left_heel", "right_heel", "left_foot_index", "right_foot_index", };
    // Start is called before the first frame update
    int fps;

    void Start()
    {
        initPosition = transform.position;
        pos_scale = 2;
    }

    public currentJoint[] Init()
    {
        Joints = new currentJoint[24];
        for (int i = 0; i < 24; i++) Joints[i] = new currentJoint();

        anim = GetComponent<Animator>();

        //Trasnform init
        Joints[(int)JointIndex.Hip].Transform = anim.GetBoneTransform(HumanBodyBones.Hips);

        Joints[(int)JointIndex.LegUp_L].Transform = anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        Joints[(int)JointIndex.LegDown_L].Transform = anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        Joints[(int)JointIndex.Foot_L].Transform = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        Joints[(int)JointIndex.Toe_L].Transform = anim.GetBoneTransform(HumanBodyBones.LeftToes);

        Joints[(int)JointIndex.LegUp_R].Transform = anim.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        Joints[(int)JointIndex.LegDown_R].Transform = anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        Joints[(int)JointIndex.Foot_R].Transform = anim.GetBoneTransform(HumanBodyBones.RightFoot);
        Joints[(int)JointIndex.Toe_R].Transform = anim.GetBoneTransform(HumanBodyBones.RightToes);

        Joints[(int)JointIndex.ArmUp_L].Transform = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        Joints[(int)JointIndex.ArmDown_L].Transform = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        Joints[(int)JointIndex.Hand_L].Transform = anim.GetBoneTransform(HumanBodyBones.LeftHand);
        Joints[(int)JointIndex.Mid_L].Transform = anim.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
        Joints[(int)JointIndex.Thumb_L].Transform = anim.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate);

        Joints[(int)JointIndex.ArmUp_R].Transform = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
        Joints[(int)JointIndex.ArmDown_R].Transform = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
        Joints[(int)JointIndex.Hand_R].Transform = anim.GetBoneTransform(HumanBodyBones.RightHand);
        Joints[(int)JointIndex.Mid_R].Transform = anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
        Joints[(int)JointIndex.Thumb_R].Transform = anim.GetBoneTransform(HumanBodyBones.RightThumbIntermediate);

        Joints[(int)JointIndex.Neck].Transform = anim.GetBoneTransform(HumanBodyBones.Neck);
        Joints[(int)JointIndex.Nose].Transform = anim.GetBoneTransform(HumanBodyBones.Head);
        Joints[(int)JointIndex.Head].Transform = anim.GetBoneTransform(HumanBodyBones.Head);
        Joints[(int)JointIndex.Eye_L].Transform = anim.GetBoneTransform(HumanBodyBones.LeftEye);
        Joints[(int)JointIndex.Eye_R].Transform = anim.GetBoneTransform(HumanBodyBones.RightEye);

        //Child init
        Joints[(int)JointIndex.LegUp_L].Child = Joints[(int)JointIndex.LegDown_L];
        Joints[(int)JointIndex.LegDown_L].Child = Joints[(int)JointIndex.Foot_L];
        Joints[(int)JointIndex.Foot_L].Child = Joints[(int)JointIndex.Toe_L];

        Joints[(int)JointIndex.LegUp_R].Child = Joints[(int)JointIndex.LegDown_R];
        Joints[(int)JointIndex.LegDown_R].Child = Joints[(int)JointIndex.Foot_R];
        Joints[(int)JointIndex.Foot_R].Child = Joints[(int)JointIndex.Toe_R];

        Joints[(int)JointIndex.ArmUp_L].Child = Joints[(int)JointIndex.ArmDown_L];
        Joints[(int)JointIndex.ArmDown_L].Child = Joints[(int)JointIndex.Hand_L];
        Joints[(int)JointIndex.Hand_L].Child = Joints[(int)JointIndex.Mid_L];

        Joints[(int)JointIndex.ArmUp_R].Child = Joints[(int)JointIndex.ArmDown_R];
        Joints[(int)JointIndex.ArmDown_R].Child = Joints[(int)JointIndex.Hand_R];
        Joints[(int)JointIndex.Hand_R].Child = Joints[(int)JointIndex.Mid_R];

        Joints[(int)JointIndex.Neck].Child = Joints[(int)JointIndex.Head];

        //Calc forward
        Vector3 a = Joints[(int)JointIndex.Hip].Transform.position;
        Vector3 b = Joints[(int)JointIndex.LegUp_L].Transform.position;
        Vector3 c = Joints[(int)JointIndex.LegUp_R].Transform.position;

        //각 캐릭터 모델에 따라 조정 필요
        forward = -1 * Vector3.Cross(a - b, a - c); //for yasmin
        // forward = Vector3.Cross(a - b, a - c); //for uniti-chan
        forward.Normalize();

        //Set Rotation & Calc Inverse
        foreach (var J in Joints)
        {
            if (J.Transform != null)
            {
                J.InitRot = J.Transform.rotation;
            }

            if (J.Child != null)
            {
                J.InitInverse = Quaternion.Inverse(Quaternion.LookRotation(J.Transform.position - J.Child.Transform.position, forward));
                J.InverseRotation = J.InitInverse * J.InitRot;
            }
        }

        var hip = Joints[(int)JointIndex.Hip];
        hip.InitInverse = Quaternion.Inverse(Quaternion.LookRotation(forward));
        hip.InverseRotation = hip.InitInverse * hip.InitRot;

        var lHand = Joints[(int)JointIndex.Hand_L];
        var lf = TriangleNormal(lHand.Transform.position, Joints[(int)JointIndex.Mid_L].Transform.position, Joints[(int)JointIndex.Thumb_L].Transform.position);
        lHand.InitRot = lHand.Transform.rotation;
        lHand.InitInverse = Quaternion.Inverse(Quaternion.LookRotation(Joints[(int)JointIndex.Hand_L].Transform.position - Joints[(int)JointIndex.Mid_L].Transform.position, lf));
        lHand.InverseRotation = lHand.InitInverse * lHand.InitRot;

        var RHand = Joints[(int)JointIndex.Hand_R];
        var Rf = TriangleNormal(RHand.Transform.position, Joints[(int)JointIndex.Thumb_R].Transform.position, Joints[(int)JointIndex.Mid_R].Transform.position);
        RHand.InitRot = RHand.Transform.rotation;
        RHand.InitInverse = Quaternion.Inverse(Quaternion.LookRotation(Joints[(int)JointIndex.Hand_R].Transform.position - Joints[(int)JointIndex.Mid_R].Transform.position, Rf));
        RHand.InverseRotation = RHand.InitInverse * RHand.InitRot;

        return Joints;
    }

    public void PoseUpdate(currentJoint[] Joints, float[,] landmark)
    {
        Joints[(int)JointIndex.LegUp_L].Pos3D = new Vector3(landmark[23, 0], landmark[23, 1], landmark[23, 2]);
        Joints[(int)JointIndex.LegDown_L].Pos3D = new Vector3(landmark[25, 0], landmark[25, 1], landmark[25, 2]);
        Joints[(int)JointIndex.Foot_L].Pos3D = new Vector3(landmark[27, 0], landmark[27, 1], landmark[27, 2]);
        Joints[(int)JointIndex.Toe_L].Pos3D = new Vector3(landmark[31, 0], landmark[31, 1], landmark[31, 2]);

        Joints[(int)JointIndex.LegUp_R].Pos3D = new Vector3(landmark[24, 0], landmark[24, 1], landmark[24, 2]);
        Joints[(int)JointIndex.LegDown_R].Pos3D = new Vector3(landmark[26, 0], landmark[26, 1], landmark[26, 2]);
        Joints[(int)JointIndex.Foot_R].Pos3D = new Vector3(landmark[28, 0], landmark[28, 1], landmark[28, 2]);
        Joints[(int)JointIndex.Toe_R].Pos3D = new Vector3(landmark[32, 0], landmark[32, 1], landmark[32, 2]);

        Joints[(int)JointIndex.ArmUp_L].Pos3D = new Vector3(landmark[11, 0], landmark[11, 1], landmark[11, 2]);
        Joints[(int)JointIndex.ArmDown_L].Pos3D = new Vector3(landmark[13, 0], landmark[13, 1], landmark[13, 2]);
        Joints[(int)JointIndex.Hand_L].Pos3D = new Vector3(landmark[15, 0], landmark[15, 1], landmark[15, 2]);
        Joints[(int)JointIndex.Mid_L].Pos3D = new Vector3((landmark[17, 0] + landmark[19, 0]) / 2, (landmark[17, 1] + landmark[19, 1]) / 2, (landmark[17, 2] + landmark[19, 2]) / 2);
        Joints[(int)JointIndex.Thumb_L].Pos3D = new Vector3(landmark[21, 0], landmark[21, 1], landmark[21, 2]);

        Joints[(int)JointIndex.ArmUp_R].Pos3D = new Vector3(landmark[12, 0], landmark[12, 1], landmark[12, 2]);
        Joints[(int)JointIndex.ArmDown_R].Pos3D = new Vector3(landmark[14, 0], landmark[14, 1], landmark[14, 2]);
        Joints[(int)JointIndex.Hand_R].Pos3D = new Vector3(landmark[16, 0], landmark[16, 1], landmark[16, 2]);
        Joints[(int)JointIndex.Mid_R].Pos3D = new Vector3((landmark[18, 0] + landmark[20, 0]) / 2, (landmark[18, 1] + landmark[20, 1]) / 2, (landmark[18, 2] + landmark[20, 2]) / 2);
        Joints[(int)JointIndex.Thumb_R].Pos3D = new Vector3(landmark[22, 0], landmark[22, 1], landmark[22, 2]);

        Joints[(int)JointIndex.Head].Pos3D = new Vector3((landmark[7, 0] + landmark[8, 0]) / 2, (landmark[7, 1] + landmark[8, 1]) / 2, (landmark[7, 2] + landmark[8, 2] + Joints[(int)JointIndex.Neck].Pos3D.z) / 3);
        Joints[(int)JointIndex.Neck].Pos3D = new Vector3((landmark[11, 0] + landmark[12, 0]) / 2, (landmark[11, 1] + landmark[12, 1]) / 2, (landmark[11, 2] + landmark[12, 2]) / 2);
        Joints[(int)JointIndex.Nose].Pos3D = new Vector3(landmark[0, 0], landmark[0, 1], landmark[0, 2]);
        Joints[(int)JointIndex.Eye_L].Pos3D = new Vector3(landmark[2, 0], landmark[2, 1], landmark[2, 2]);
        Joints[(int)JointIndex.Eye_R].Pos3D = new Vector3(landmark[5, 0], landmark[5, 1], landmark[5, 2]);

        Joints[(int)JointIndex.Hip].Pos3D = (((Joints[(int)JointIndex.LegUp_L].Pos3D + Joints[(int)JointIndex.LegUp_R].Pos3D) / 2f) + Joints[(int)JointIndex.Neck].Pos3D) / 2f;

        forward = TriangleNormal(Joints[(int)JointIndex.Neck].Pos3D, Joints[(int)JointIndex.LegUp_L].Pos3D, Joints[(int)JointIndex.LegUp_R].Pos3D);

        Joints[(int)JointIndex.Hip].Transform.rotation = Quaternion.LookRotation(forward) * Joints[(int)JointIndex.Hip].InverseRotation;
        //Joints[(int)JointIndex.Hip].Transform.position += (new Vector3(Convert.ToSingle(landmark[33, 0]), Convert.ToSingle(landmark[33, 1]), landmark[33, 2]) - prevPosition) * pos_scale;
        transform.position += (new Vector3(Convert.ToSingle(landmark[33, 0]), Convert.ToSingle(landmark[33, 1]), landmark[33, 2]) - prevPosition) * pos_scale;
        prevPosition = new Vector3(Convert.ToSingle(landmark[33, 0]), Convert.ToSingle(landmark[33, 1]), landmark[33, 2]);

        foreach (var J in Joints)
        {
            if (J.Child != null)
            {
                J.Transform.rotation = Quaternion.LookRotation(J.Pos3D - J.Child.Pos3D, forward) * J.InverseRotation;
            }
        }

        var lHand = Joints[(int)JointIndex.Hand_L];
        var lf = TriangleNormal(lHand.Pos3D, Joints[(int)JointIndex.Mid_L].Pos3D, Joints[(int)JointIndex.Thumb_L].Pos3D);
        lHand.Transform.rotation = Quaternion.LookRotation(Joints[(int)JointIndex.Hand_L].Pos3D - Joints[(int)JointIndex.Mid_L].Pos3D, lf) * lHand.InverseRotation;

        var RHand = Joints[(int)JointIndex.Hand_R];
        var Rf = TriangleNormal(RHand.Pos3D, Joints[(int)JointIndex.Thumb_R].Pos3D, Joints[(int)JointIndex.Mid_R].Pos3D);
        RHand.Transform.rotation = Quaternion.LookRotation(Joints[(int)JointIndex.Hand_R].Pos3D - Joints[(int)JointIndex.Mid_R].Pos3D, Rf) * RHand.InverseRotation;

    }

    Vector3 TriangleNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 d1 = a - b;
        Vector3 d2 = a - c;

        Vector3 dd = Vector3.Cross(d1, d2);
        dd.Normalize();

        return dd;
    }

    private Quaternion GetInverse(GameObject p1, GameObject p2, Vector3 forward)
    {
        return Quaternion.Inverse(Quaternion.LookRotation(p1.transform.position - p2.transform.position, forward));
    }

    public void set_initpos(float[,] landmark)
    {
        prevPosition = new Vector3(Convert.ToSingle(landmark[33, 0]), Convert.ToSingle(landmark[33, 1]), landmark[33, 2]);
    }
}
