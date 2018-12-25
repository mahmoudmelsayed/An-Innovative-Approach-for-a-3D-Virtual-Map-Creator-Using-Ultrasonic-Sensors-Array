#define M1                                          4
#define E1                                          5
#define M2                                          7
#define E2                                          6
#define SPEED_FULL                                  255
#define SPEED_HALF                                  127
#define SPEED_QUAR                                  63
#define MOVE_DELAY                                  50
#define MOVE_DELAY_BACK                             150

#define SERIAL_START_CHAR                           '$'
#define SERIAL_STOP_CHAR                            '*'
#define SERIAL_INVALID                              "INVALID"
#define SERIAL_CMD_CONNECT                          "CONNECT"
#define SERIAL_CMD_DISCONNECT                       "DISCONNECT"
#define SERIAL_CMD_START                            "START"
#define SERIAL_CMD_STOP                             "STOP"
#define SERIAL_CMD_GET_MAP_DATA                     "GET_MAP_DATA"
#define SERIAL_CMD_GET_SETTINGS_SPEED               "GET_SETTINGS_SPEED"
#define SERIAL_CMD_GET_SETTINGS_MIN_DISTANCE        "GET_SETTINGS_MIN_DISTANCE"
#define SERIAL_CMD_SET_SETTINGS_SPEED               "SET_SETTINGS_SPEED"
#define SERIAL_CMD_SET_SETTINGS_MIN_DISTANCE        "SET_SETTINGS_MIN_DISTANCE"
#define SERIAL_CMDRSPN_CONNECT                      "TEST-CAR-1"
#define SERIAL_CMDRSPN_DISCONNECT                   "DISCONNECTED"
#define SERIAL_CMDRSPN_SUCCESS                      "SUCCESS"
#define SERIAL_CMDRSPN_GET_MAP_DATA_000_DEG         "MAP_DATA_000_DEG"
#define SERIAL_CMDRSPN_GET_MAP_DATA_045_DEG         "MAP_DATA_045_DEG"
#define SERIAL_CMDRSPN_GET_MAP_DATA_090_DEG         "MAP_DATA_090_DEG"
#define SERIAL_CMDRSPN_GET_MAP_DATA_135_DEG         "MAP_DATA_135_DEG"
#define SERIAL_CMDRSPN_GET_MAP_DATA_180_DEG         "MAP_DATA_180_DEG"
#define SERIAL_CMDRSPN_GET_SETTINGS_SPEED           "SETTINGS_SPEED"
#define SERIAL_CMDRSPN_GET_SETTINGS_MIN_DISTANCE    "SETTINGS_MIN_DISTANCE"

#define ECHO_PIN1                                   23          // center
#define TRIGGER_PIN1                                25          // center
#define ECHO_PIN2                                   27          // right
#define TRIGGER_PIN2                                29          // right
#define ECHO_PIN3                                   31          // left
#define TRIGGER_PIN3                                33          // left
#define ECHO_PIN4                                   35          // 0 deg
#define TRIGGER_PIN4                                37          // 0 deg
#define ECHO_PIN5                                   39          // 45 deg
#define TRIGGER_PIN5                                41          // 45 deg
#define ECHO_PIN6                                   43          // 90 deg
#define TRIGGER_PIN6                                45          // 90 deg
#define ECHO_PIN7                                   A13         // 135 deg
#define TRIGGER_PIN7                                A12         // 135 deg
#define ECHO_PIN8                                   A15         // 180 deg
#define TRIGGER_PIN8                                A14         // 180 deg
#define MAX_DISTANCE                                500

#define SETTINGS_SPEED_EEPROM_LOCATION              0
#define SETTINGS_MIN_DISTANCE_EEPROM_LOCATION       10
#define SETTINGS_SPEED_MIN_VALUE                    80
#define SETTINGS_SPEED_MAX_VALUE                    250
#define SETTINGS_MIN_DISTANCE_MIN_VALUE             10
#define SETTINGS_MIN_DISTANCE_MAX_VALUE             50


#include <EEPROM.h>
#include <NewPing.h>

enum drive_direction_t
{
    Forward,
    Backward,
    Right,
    Left
};

bool is_connected = false;
bool is_moving = false;
int count = 0;
byte motor_speed = 127;
byte min_distance = 25;

NewPing sonar_center(TRIGGER_PIN1, ECHO_PIN1, MAX_DISTANCE);
NewPing sonar_right(TRIGGER_PIN2, ECHO_PIN2, MAX_DISTANCE);
NewPing sonar_left(TRIGGER_PIN3, ECHO_PIN3, MAX_DISTANCE);
NewPing sonar_000_deg(TRIGGER_PIN4, ECHO_PIN4, MAX_DISTANCE);
NewPing sonar_045_deg(TRIGGER_PIN5, ECHO_PIN5, MAX_DISTANCE);
NewPing sonar_090_deg(TRIGGER_PIN6, ECHO_PIN6, MAX_DISTANCE);
NewPing sonar_135_deg(TRIGGER_PIN7, ECHO_PIN7, MAX_DISTANCE);
NewPing sonar_180_deg(TRIGGER_PIN8, ECHO_PIN8, MAX_DISTANCE);

void setup()
{
    // setup serial port
    Serial1.begin(9600);
    
    // wait for serial to ready
    while (!Serial1);

    // check EEPROM
    motor_speed = EEPROM.read(SETTINGS_SPEED_EEPROM_LOCATION);
    if (motor_speed == 255)
    {
        motor_speed = 127;
        EEPROM.write(SETTINGS_SPEED_EEPROM_LOCATION, 127);
    }

    min_distance = EEPROM.read(SETTINGS_MIN_DISTANCE_EEPROM_LOCATION);
    if (min_distance == 255)
    {
        min_distance = 25;
        EEPROM.write(SETTINGS_MIN_DISTANCE_EEPROM_LOCATION, 25);
    }
}

void loop()
{
    if (Serial1.available() > 0)
    {
        String cmd = Serial1.readStringUntil(SERIAL_STOP_CHAR);

        unsigned int length = cmd.length();
        unsigned int start_index = 0;
        bool valid_cmd = false;

        for (start_index = 0; start_index < length; start_index++)
        {
            if (cmd[start_index] == SERIAL_START_CHAR)
            {
                valid_cmd = true;
                break;
            }
        }

        if (!valid_cmd)
        {
            return;
        }

        start_index++;
        String cmd2 = "";
        for (int i = start_index; i < length; i++)
        {
            cmd2.concat(cmd[i]);
        }

        interpret_cmd(cmd2);
    }

    if (is_moving)
    {
        auto_move();
    }
}

void interpret_cmd(String cmd)
{
    if (cmd == SERIAL_CMD_CONNECT)
    {
        is_connected = true;
        Serial1.print(SERIAL_START_CHAR);
        Serial1.print(SERIAL_CMDRSPN_CONNECT);
        Serial1.print(SERIAL_STOP_CHAR);
    }

    else if (cmd == SERIAL_CMD_DISCONNECT)
    {
        is_connected = false;
        Serial1.print(SERIAL_START_CHAR);
        Serial1.print(SERIAL_CMDRSPN_DISCONNECT);
        Serial1.print(SERIAL_STOP_CHAR);
    }

    if (is_connected)
    {
        if (cmd == SERIAL_CMD_START)
        {
            is_moving = true;
            Serial1.print(SERIAL_START_CHAR);
            Serial1.print(SERIAL_CMDRSPN_SUCCESS);
            Serial1.print(SERIAL_STOP_CHAR);
        }
    
        else if (cmd == SERIAL_CMD_STOP)
        {
            is_moving = false;
            stop();
            Serial1.print(SERIAL_START_CHAR);
            Serial1.print(SERIAL_CMDRSPN_SUCCESS);
            Serial1.print(SERIAL_STOP_CHAR);
        }
    
        else if (cmd == SERIAL_CMD_GET_MAP_DATA)
        {
            send_map_data(count);
            count++;
        }
    
        else if (cmd == SERIAL_CMD_GET_SETTINGS_SPEED)
        {
            Serial1.print(SERIAL_START_CHAR);
            Serial1.print(SERIAL_CMDRSPN_GET_SETTINGS_SPEED);
            Serial1.print(SERIAL_STOP_CHAR);

            Serial1.print(SERIAL_START_CHAR);
            Serial1.print(motor_speed);
            Serial1.print(SERIAL_STOP_CHAR);
        }
    
        else if (cmd == SERIAL_CMD_GET_SETTINGS_MIN_DISTANCE)
        {
            Serial1.print(SERIAL_START_CHAR);
            Serial1.print(SERIAL_CMDRSPN_GET_SETTINGS_MIN_DISTANCE);
            Serial1.print(SERIAL_STOP_CHAR);
            
            Serial1.print(SERIAL_START_CHAR);
            Serial1.print(min_distance);
            Serial1.print(SERIAL_STOP_CHAR);
        }
    
        else if (cmd == SERIAL_CMD_SET_SETTINGS_SPEED)
        {
            String data = read_serial();
            if (data == SERIAL_INVALID)
            {
                Serial1.print(SERIAL_START_CHAR);
                Serial1.print(SERIAL_INVALID);
                Serial1.print(SERIAL_STOP_CHAR);
            }

            else
            {
                long val = data.toInt();
                if (val < SETTINGS_SPEED_MIN_VALUE || val > SETTINGS_SPEED_MAX_VALUE)
                {
                    Serial1.print(SERIAL_START_CHAR);
                    Serial1.print(SERIAL_INVALID);
                    Serial1.print(SERIAL_STOP_CHAR);
                }

                else
                {
                    byte new_val = (byte)val;
                    EEPROM.write(SETTINGS_SPEED_EEPROM_LOCATION, new_val);
                    motor_speed = new_val;

                    Serial1.print(SERIAL_START_CHAR);
                    Serial1.print(SERIAL_CMDRSPN_SUCCESS);
                    Serial1.print(SERIAL_STOP_CHAR);
                }
            }
        }
    
        else if (cmd == SERIAL_CMD_SET_SETTINGS_MIN_DISTANCE)
        {
            String data = read_serial();
            if (data == SERIAL_INVALID)
            {
                Serial1.print(SERIAL_START_CHAR);
                Serial1.print(SERIAL_INVALID);
                Serial1.print(SERIAL_STOP_CHAR);
            }

            else
            {
                long val = data.toInt();
                if (val < SETTINGS_MIN_DISTANCE_MIN_VALUE || val > SETTINGS_MIN_DISTANCE_MAX_VALUE)
                {
                    Serial1.print(SERIAL_START_CHAR);
                    Serial1.print(SERIAL_INVALID);
                    Serial1.print(SERIAL_STOP_CHAR);
                }

                else
                {
                    byte new_val = (byte)val;
                    EEPROM.write(SETTINGS_MIN_DISTANCE_EEPROM_LOCATION, new_val);
                    min_distance = new_val;

                    Serial1.print(SERIAL_START_CHAR);
                    Serial1.print(SERIAL_CMDRSPN_SUCCESS);
                    Serial1.print(SERIAL_STOP_CHAR);
                }
            }
        }
    }
}

void auto_move()
{
    double dist_center = sonar_center.ping_cm();
    double dist_right = sonar_right.ping_cm();
    double dist_left = sonar_left.ping_cm();
    // debug(dist_center);
    // debug(dist_right);
    // debug(dist_left);
    if (dist_center == 0)
    {
        dist_center = MAX_DISTANCE;
    }
    if (dist_right == 0)
    {
        dist_right = MAX_DISTANCE;
    }
    if (dist_left == 0)
    {
        dist_left = MAX_DISTANCE;
    }

    if (dist_center > min_distance)
    {
        if (dist_left < min_distance && dist_right < min_distance)
        {
            move(Forward);
            delay(MOVE_DELAY);
        }

        else if (dist_left < min_distance)
        {
            move(Right);
            delay(MOVE_DELAY);
        }

        else if (dist_right < min_distance)
        {
            move(Left);
            delay(MOVE_DELAY);
        }
        
        else
        {
            move(Forward);
            delay(MOVE_DELAY);
        }
    }

    else
    {
        if (dist_left < min_distance && dist_right < min_distance)
        {
            move(Backward);
            delay(MOVE_DELAY_BACK);
        }

        else if (dist_left < min_distance)
        {
            move(Backward);
            delay(MOVE_DELAY_BACK);
            move(Right);
            delay(MOVE_DELAY);
        }

        else if (dist_right < min_distance)
        {
            move(Backward);
            delay(MOVE_DELAY_BACK);
            move(Left);
            delay(MOVE_DELAY);
        }
        
        else
        {
            move(Backward);
            delay(MOVE_DELAY_BACK);
        }
    }
}

void send_map_data(int id)
{
    double dist_000_deg = sonar_000_deg.ping_cm();
    double dist_045_deg = sonar_045_deg.ping_cm();
    double dist_090_deg = sonar_090_deg.ping_cm();
    double dist_135_deg = sonar_135_deg.ping_cm();
    double dist_180_deg = sonar_180_deg.ping_cm();
    
    // convert to string
    String id_str(id);
    String dist_000_deg_str(dist_000_deg, 2);
    String dist_045_deg_str(dist_045_deg, 2);
    String dist_090_deg_str(dist_090_deg, 2);
    String dist_135_deg_str(dist_135_deg, 2);
    String dist_180_deg_str(dist_180_deg, 2);


    // 000 degree
    Serial1.print(SERIAL_START_CHAR);
    Serial1.print(SERIAL_CMDRSPN_GET_MAP_DATA_000_DEG);
    Serial1.print(SERIAL_STOP_CHAR);
    Serial1.print(SERIAL_START_CHAR);
    Serial1.print(id_str);
    Serial1.print(SERIAL_STOP_CHAR);
    Serial1.print(SERIAL_START_CHAR);
    Serial1.print(dist_000_deg_str);
    Serial1.print(SERIAL_STOP_CHAR);


    // 045 degree
    Serial1.print(SERIAL_START_CHAR);
    Serial1.print(SERIAL_CMDRSPN_GET_MAP_DATA_045_DEG);
    Serial1.print(SERIAL_STOP_CHAR);
    Serial1.print(SERIAL_START_CHAR);
    Serial1.print(id_str);
    Serial1.print(SERIAL_STOP_CHAR);
    Serial1.print(SERIAL_START_CHAR);
    Serial1.print(dist_045_deg_str);
    Serial1.print(SERIAL_STOP_CHAR);


    // 090 degree
    Serial1.print(SERIAL_START_CHAR);
    Serial1.print(SERIAL_CMDRSPN_GET_MAP_DATA_090_DEG);
    Serial1.print(SERIAL_STOP_CHAR);
    Serial1.print(SERIAL_START_CHAR);
    Serial1.print(id_str);
    Serial1.print(SERIAL_STOP_CHAR);
    Serial1.print(SERIAL_START_CHAR);
    Serial1.print(dist_090_deg_str);
    Serial1.print(SERIAL_STOP_CHAR);


    // 135 degree
    Serial1.print(SERIAL_START_CHAR);
    Serial1.print(SERIAL_CMDRSPN_GET_MAP_DATA_135_DEG);
    Serial1.print(SERIAL_STOP_CHAR);
    Serial1.print(SERIAL_START_CHAR);
    Serial1.print(id_str);
    Serial1.print(SERIAL_STOP_CHAR);
    Serial1.print(SERIAL_START_CHAR);
    Serial1.print(dist_135_deg_str);
    Serial1.print(SERIAL_STOP_CHAR);


    // 180 degree
    Serial1.print(SERIAL_START_CHAR);
    Serial1.print(SERIAL_CMDRSPN_GET_MAP_DATA_180_DEG);
    Serial1.print(SERIAL_STOP_CHAR);
    Serial1.print(SERIAL_START_CHAR);
    Serial1.print(id_str);
    Serial1.print(SERIAL_STOP_CHAR);
    Serial1.print(SERIAL_START_CHAR);
    Serial1.print(dist_180_deg_str);
    Serial1.print(SERIAL_STOP_CHAR);
}

void move(drive_direction_t dir)
{
    switch (dir)
    {
        case Forward:
            digitalWrite(M1, LOW);
            digitalWrite(M2, LOW);
            analogWrite(E1, motor_speed);
            analogWrite(E2, motor_speed);
            break;
            
        case Backward:
            digitalWrite(M1, HIGH);
            digitalWrite(M2, HIGH);
            analogWrite(E1, motor_speed);
            analogWrite(E2, motor_speed);
            break;

        case Right:
            digitalWrite(M1, HIGH);
            digitalWrite(M2, LOW);
            analogWrite(E1, SPEED_HALF);
            analogWrite(E2, SPEED_HALF);
            break;

        case Left:
            digitalWrite(M1, LOW);
            digitalWrite(M2, HIGH);
            analogWrite(E1, SPEED_HALF);
            analogWrite(E2, SPEED_HALF);
            break;
  }
}

void stop()
{
    analogWrite(E1, 0);
    analogWrite(E2, 0);
}

void debug(double val)
{
    String temp(val, 2);
    Serial1.print(temp);
}

String read_serial()
{
    String data = Serial1.readStringUntil(SERIAL_STOP_CHAR);

    unsigned int length = data.length();
    unsigned int start_index = 0;
    bool valid_data = false;
        
    for (start_index = 0; start_index < length; start_index++)
    {
        if (data[start_index] == SERIAL_START_CHAR)
        {
            valid_data = true;
            break;
        }
    }
    
    if (!valid_data)
    {
        return SERIAL_INVALID;
    }
    
    start_index++;
    String data_str = "";
    for (int i = start_index; i < length; i++)
    {
        data_str.concat(data[i]);
    }

    return data_str;
}