import cv2
import numpy as np
import socket

# ---------- SETTINGS ----------
width, height = 1280, 720
# color_lower = np.array([35, 111, 111])   # GREEN
# color_upper = np.array([80, 255, 255])

red_lower1 = np.array([0, 100, 60])
red_upper1 = np.array([15, 255, 255])

red_lower2 = np.array([165, 100, 60])
red_upper2 = np.array([180, 255, 255])
alpha = 0.3

# ---------- CAMERA SETUP ----------
cap = cv2.VideoCapture(0)
cap.set(3, width)
cap.set(4, height)

# ---------- UDP SETUP ----------
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
serverAddressPort = ("127.0.0.1", 5055)  # must match Unity port

# ---------- VARIABLES ----------
prev_cx, prev_cy, prev_z = None, None, None

while True:
    success, img = cap.read()
    if not success:
        break

    # Convert to HSV
    hsv = cv2.cvtColor(img, cv2.COLOR_BGR2HSV)

    # Create mask

    # GREEN
    # mask = cv2.inRange(hsv, color_lower, color_upper)

    # RED
    mask1 = cv2.inRange(hsv, red_lower1, red_upper1)
    mask2 = cv2.inRange(hsv, red_lower2, red_upper2)
    mask = cv2.bitwise_or(mask1, mask2)

    # Find contours
    contours, _ = cv2.findContours(mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

    if contours:
        cnt = max(contours, key=cv2.contourArea)
        area = cv2.contourArea(cnt)
        if area > 500:  # ignore tiny dots
            x, y, w, h = cv2.boundingRect(cnt)
            cx, cy = x + w // 2, y + h // 2

            # --- FAKE DEPTH ---
            z = np.clip(100000 / area, 0, 200)

            # --- SMOOTH MOTION ---
            if prev_cx is not None:
                cx = int(prev_cx * (1 - alpha) + cx * alpha)
                cy = int(prev_cy * (1 - alpha) + cy * alpha)
                z = prev_z * (1 - alpha) + z * alpha

            prev_cx, prev_cy, prev_z = cx, cy, z

            # --- CONVERT TO UNITY SPACE ---
            center_x, center_y = width // 2, height // 2
            x_unity = (cx - center_x) / 100.0       # left negative, right positive
            y_unity = -(cy - center_y) / 100.0      # up positive
            z_unity = (z - 100) / 10.0              # center around 0

            # --- DRAW FEEDBACK ---
            cv2.circle(img, (cx, cy), 10, (0, 255, 0), cv2.FILLED)
            cv2.putText(img, f"X:{x_unity:.2f} Y:{y_unity:.2f} Z:{z_unity:.2f}",
                        (cx + 20, cy - 20), cv2.FONT_HERSHEY_SIMPLEX, 0.7, (0, 255, 0), 2)

            # --- SEND TO UNITY ---
            message = f"{x_unity:.2f},{y_unity:.2f},{z_unity:.2f}"
            sock.sendto(message.encode(), serverAddressPort)

            print(message)

    # Smaller display windows
    display = cv2.resize(img, (800, 450))
    maskDisplay = cv2.resize(mask, (800, 450))

    cv2.imshow("Image", display)
    cv2.imshow("Mask", maskDisplay)

    # Quit with Q key
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# ---------- CLEANUP ----------
cap.release()
cv2.destroyAllWindows()
