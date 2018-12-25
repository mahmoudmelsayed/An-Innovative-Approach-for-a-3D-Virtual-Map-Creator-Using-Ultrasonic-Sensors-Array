# An-Innovative-Approach-for-a-3D-Virtual-Map-Creator-Using-Ultrasonic-Sensors-Array
## Breif Description:

This project is a development of a 3D mapper robot. It consists of two main parts, the Arduino to control the vehicle and to send the data to the computer where the GUI that was built by C# will take over and plot the 3D map based on the received data 

## CONTROL ALGORITHM
### A.	The robot vehicle
The car moves based on the range of speed selected by the user and this is achieved by a simple torque control algorithm that keeps the range in the desired speed through the motor driver control. 
	Once the speed range is selected and the start button is pressed the car will begin its motion and the three front sensors L, R and M will start recording signals and communicating with the Arduino to detect any possible obstacles and so for these sensors there are four possible scenarios at minimum. 
	Firstly, in the case where the right hand side sensor (sensor R) detects an object and the robot will be set to turn left to avoid that obstacle following the order of an encoded program saved in the Arduino memory. And this obstacle avoiding strategy is set by default and the user has no control over it to ensure the safety of the robot. 
	Similarly for the second scenario when the left hand side sensor (sensor L) detects an obstacle, the vehicle will turn right to avoid it. 
	Third scenario is if both the right and left hand side sensors (both sensor L and R) detect an obstacle simultaneously, then the reading of the middle sensor will set the decision whether the path in front is clear from obstacle or not, if it's clear then the robot will keep on moving forward. 
	The fourth scenario is if all front row sensors (sensor L, R and M) detect an obstacle simultaneously, stating that the program will interpret as a dead end and hence stop the robot forward motion. 
	It is also important to mention that even though the navigation strategy mentioned above is by default. The user can actually determine a value for the minimum distance between the obstacle and the robot at which this default navigation algorithm is activated. 

### B.	The 3D mapping Algorithm
	The 3D mapping data is obtained for the 2D plane firstly, the X and Z coordinate, from the 5 ultrasonic sensors mounted on the arc. The value for the third axis (Y- axis) is obtained from the position of the robot on its forward track. 
Once the car moving start, the 5 sensors starts detecting for the X and Z values and the counter starts incrementing for the Y values. 
	The ultrasonic sensor records time values and those values are substituted in the equation (1) to obtain the values of the distance. This calculation is performed by certain portion of the code in Arduino. The resulting distance values obtained are then encoded and sent to the X-bee which in turn sends it to the receiving XBee attached to the computer. 
Distance = Speed * Time/2	(1)
When the second XBee receives the data, plot the 3D map. All of these features will be displayed on the GUI developed through C# as well within the same program flow. 

### Pictures below show the robot, the components of the system, the GUI outlook, and the sensors  distribution.

![image](https://user-images.githubusercontent.com/44964071/50421745-a4f29f00-083a-11e9-898f-df5b86e00f8f.png)

![image](https://user-images.githubusercontent.com/44964071/50421787-f00cb200-083a-11e9-96b1-36488f7e386d.png)

![image](https://user-images.githubusercontent.com/44964071/50421789-fa2eb080-083a-11e9-8524-e1a992a49eb0.png)

![image](https://user-images.githubusercontent.com/44964071/50421803-15012500-083b-11e9-955f-6985e7b58471.png)

![image](https://user-images.githubusercontent.com/44964071/50421805-1b8f9c80-083b-11e9-93b5-ea2c4dbb8843.png)


