import cv2
import mediapipe as mp
import timeit
mp_drawing = mp.solutions.drawing_utils
mp_drawing_styles = mp.solutions.drawing_styles
mp_pose = mp.solutions.pose


# For webcam input:
path="Assets/source/"
path_="Assets/result/"

vid_name = input("wait for video name...")
#vid_name = "p_video"
print("video name input success")

cap = cv2.VideoCapture(path+vid_name+".mp4")

fps = cap.get(cv2.CAP_PROP_FPS)
w = round(cap.get(cv2.CAP_PROP_FRAME_WIDTH))
h = round(cap.get(cv2.CAP_PROP_FRAME_HEIGHT))
delay = round(1000/fps)
print("video open success")

fourcc = cv2.VideoWriter_fourcc(*'mp4v')

out=cv2.VideoWriter(path_+vid_name+"_landmark.mp4",fourcc,fps,(w,h))
f=open(path_+vid_name+'_landmarks.txt','w')

init_pos=True
image_width = 1280
image_height=720
start_time = timeit.default_timer()

print("mediapipe process...")
with mp_pose.Pose(
    min_detection_confidence=0.5,
    min_tracking_confidence=0.5) as pose:
  while cap.isOpened():

    success, image = cap.read()
    if not success:
      print("Ignoring empty camera frame.")
      # If loading a video, use 'break' instead of 'continue'.
      break

    # To improve performance, optionally mark the image as not writeable to
    # pass by reference.
    image.flags.writeable = False
    image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    results = pose.process(image)
    hip_x=(results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_HIP].x +results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_HIP].x) /2
    hip_y=(results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_HIP].y +results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_HIP].y) /2
    hip_z=0.0

    keypoints = []
    print(results.pose_world_landmarks.landmark)
    for i, data_point in enumerate(results.pose_world_landmarks.landmark):
        keypoints.append({
            data_point.x,
            data_point.y,
            data_point.z
        })
    keypoints.append([hip_x,
                      hip_y,
                      hip_z])

    #delay = timeit.default_timer() - start_time
    #print("delay = ", delay)
    f.write(str(delay)+","+str(keypoints) + '\n')
    start_time = timeit.default_timer()

    # Draw the pose annotation on the image.
    image.flags.writeable = True
    image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)
    image = cv2.line(image, (int(hip_x),int(hip_y)),(int(hip_x),int(hip_y)),(255,0,0),50)
    mp_drawing.draw_landmarks(
        image,
        results.pose_landmarks,
        mp_pose.POSE_CONNECTIONS,
        landmark_drawing_spec=mp_drawing_styles.get_default_pose_landmarks_style())

    # Flip the image and 3Dpose horizontally for a selfie-view display.
    cv2.imshow('MediaPipe Pose', cv2.flip(image, 1))
    out.write(cv2.flip(image, 1))
    if cv2.waitKey(5) & 0xFF == 27:
      break
cap.release()
f.close()