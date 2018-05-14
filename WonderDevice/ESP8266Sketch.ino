/*
 Basic ESP8266 MQTT example
   This sketch demonstrates the capabilities of the pubsub library in combination
   with the ESP8266 board/library.
 It connects to an MQTT server then:
  - publishes "hello world" to the topic "outTopic" every two seconds
  - subscribes to the topic "inTopic", printing out any messages
    it receives. NB - it assumes the received payloads are strings not binary
  - If the first character of the topic "inTopic" is an 1, switch ON the ESP Led,
    else switch it off
 It will reconnect to the server if the connection is lost using a blocking
 reconnect function. See the 'mqtt_reconnect_nonblocking' example for how to
 achieve the same result without blocking the main loop.

 To install the ESP8266 board, (using Arduino 1.6.4+):
  - Add the following 3rd party board manager under "File -> Preferences -> Additional Boards Manager URLs":
       http://arduino.esp8266.com/stable/package_esp8266com_index.json
  - Open the "Tools -> Board -> Board Manager" and click install for the ESP8266"
  - Select your ESP8266 in "Tools -> Board"
*/
#include <ESP8266WiFi.h>
#include <PubSubClient.h>

// Update these with values suitable for your network.
//const char* ssid = "wifissid2";
//const char* password = "wifipassword2";
//const char* mqtt_server = "192.168.1.218";
const char* ssid = "wifissid";
const char* password = "wifipassword";
const char* mqtt_server = "172.20.10.2";

const char* heirnamet0 =  "travel/silverbox/temperature/0/pv";
const char* heirnamet1 =  "travel/silverbox/temperature/1/pv";
const char* heirnamedt =  "travel/silverbox/temperature/delta/pv";//dt=t0-t1
const char* heirnamedistance =  "travel/silverbox/distance/pv";// distance in cm
const char* heirnamecount =  "travel/silverbox/count/pv";// 999 rollover counter (passing parts, people
const int   ultrarollovercount = 1000;
long ultradistancelastMsg = 0;
long ultracountermaxdistance = 30; //30 cm or closer constitutes an object passing

WiFiClient espClient;
PubSubClient client(espClient);
long temperatureslastMsg = 0;
char msg[50];
int value = 0;

long now; //will contain milliseconds

//dallas onewire two temperature 
char temp0Fchar[6];
float temp0F = -999;
char temp1Fchar[6];
float temp1F = -999;
float difftemp0minus1F = -999;
char difftemp0minus1Fchar[6];
char prevtemp0Fchar[6];
char prevtemp1Fchar[6];
int ret = 0;
bool temperaturesupdaterequiredflag = false;
// Include the libraries we need

#include <OneWire.h>
#include <DallasTemperature.h>
// Data wire is plugged into port 2 on the Arduino
#define ONE_WIRE_BUS 2
#define TEMPERATURE_PRECISION 9
// Setup a oneWire instance to communicate with any OneWire devices (not just Maxim/Dallas temperature ICs)
OneWire oneWire(ONE_WIRE_BUS);
// Pass our oneWire reference to Dallas Temperature. 
DallasTemperature sensors(&oneWire);
//float prevtempF = -999;
// arrays to hold device addresses
DeviceAddress thermometer0, thermometer1;

//ultrasonic distance and rollover counter
#define ultratrigPin 5
#define ultraechoPin 4
int ultracounter = 0;
int ultracurrentState = 0;
int ultrapreviousState = 0;
long ultraduration, ultradistance;
long ultrapreviousdistance = 0;
long ultradistancediff = 0;
bool ultraupdaterequiredflag = false;
char ultracounterchar[6];
char ultradistancechar[6];

void setup()  
{ 
  Serial.begin(115200);

  // requestTemperatures() will not block current thread
  sensors.setWaitForConversion(false);

  pinMode(BUILTIN_LED, OUTPUT);     // Initialize the BUILTIN_LED pin as an output
  setup_wifi();
  client.setServer(mqtt_server, 1883);
//  client.setCallback(callback);

  // Start up the library (dallas)
  sensors.begin();

    // locate devices on the bus
  Serial.print("Locating devices...");
  Serial.print("Found ");
  Serial.print(sensors.getDeviceCount(), DEC);
  Serial.println(" devices.");

  // report parasite power requirements
  Serial.print("Parasite power is: ");
  if (sensors.isParasitePowerMode()) Serial.println("ON");
  else Serial.println("OFF");

  if (!sensors.getAddress(thermometer0, 0)) Serial.println("Unable to find address for Device thermometer0");
  if (!sensors.getAddress(thermometer1, 1)) Serial.println("Unable to find address for Device thermometer1");

  // show the addresses we found on the bus
  Serial.print("Device 0 Address: ");
  printAddress(thermometer0);
  Serial.println();

  Serial.print("Device 1 Address: ");
  printAddress(thermometer1);
  Serial.println();

  // set the resolution to 9 bit per device
  sensors.setResolution(thermometer0, TEMPERATURE_PRECISION);
  sensors.setResolution(thermometer1, TEMPERATURE_PRECISION);

  Serial.print("Device 0 Resolution: ");
  Serial.print(sensors.getResolution(thermometer0), DEC);
  Serial.println();

  Serial.print("Device 1 Resolution: ");
  Serial.print(sensors.getResolution(thermometer1), DEC);
  Serial.println();

  //ultrasonic distance and rollover counter
  pinMode(ultratrigPin, OUTPUT);
  pinMode(ultraechoPin, INPUT);
}

void setup_wifi() {
  delay(10);
  // We start by connecting to a WiFi network
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  randomSeed(micros());
  Serial.println("");
  Serial.println("WiFi connected");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());
}

/*
void callback(char* topic, byte* payload, unsigned int length) {
  Serial.print("Message arrived [");
  Serial.print(topic);
  Serial.print("] ");
  for (int i = 0; i < length; i++) {
    Serial.print((char)payload[i]);
  }
  Serial.println();

  // Switch on the LED if an 1 was received as first character
  if ((char)payload[0] == '1') {
    digitalWrite(BUILTIN_LED, LOW);   // Turn the LED on (Note that LOW is the voltage level
    // but actually the LED is on; this is because
    // it is acive low on the ESP-01)
  } else {
    digitalWrite(BUILTIN_LED, HIGH);  // Turn the LED off by making the voltage HIGH
  }
}
*/

void reconnect() {
  // Loop until we're reconnected
  while (!client.connected()) {
    Serial.print("Attempting MQTT connection...");
    // Create a random client ID
    String clientId = "ESP8266Client-";
    clientId += String(random(0xffff), HEX);
    // Attempt to connect
    if (client.connect(clientId.c_str())) {
      Serial.println("connected");
      // Once connected, publish an announcement...
      client.publish("outTopic", "hello world");
      // ... and resubscribe
      client.subscribe("inTopic");
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      // Wait 5 seconds before retrying
      delay(5000);
    }
  }
}

// more dallas functions
// dallas function to print a device address
void printAddress(DeviceAddress deviceAddress)
{
  for (uint8_t i = 0; i < 8; i++)
  {
    // zero pad the address if necessary
    if (deviceAddress[i] < 16) Serial.print("0");
    Serial.print(deviceAddress[i], HEX);
  }
}

// dallas function to print the temperature for a device
void printTemperature(DeviceAddress deviceAddress)
{
  float tempC = sensors.getTempC(deviceAddress);
  Serial.print("Temp C: ");
  Serial.print(tempC);
  Serial.print(" Temp F: ");
  Serial.print(DallasTemperature::toFahrenheit(tempC));
}

// dallas function to print a device's resolution
void printResolution(DeviceAddress deviceAddress)
{
  Serial.print("Resolution: ");
  Serial.print(sensors.getResolution(deviceAddress));
  Serial.println();
}

// dallas main function to print information about a device
void printData(DeviceAddress deviceAddress)
{
  Serial.print("Device Address: ");
  printAddress(deviceAddress);
  Serial.print(" ");
  printTemperature(deviceAddress);
  Serial.println();
}

void loop() {
  if (!client.connected()) {
    reconnect();
  }
  client.loop();

  now = millis();

//  Serial.print(now);   Serial.print(" ");  Serial.print(temperaturesupdaterequiredflag);  Serial.print(" ");  Serial.print(temperatureslastMsg);  Serial.print(" ");

  temperaturesupdaterequiredflag = false;
  if (now - temperatureslastMsg > 2000) {
    temperatureslastMsg = now;
  
    // Fetch temperatures from Dallas sensors
    // call sensors.requestTemperatures() to issue a global temperature 
    // request to all devices on the bus
    //Serial.print("Requesting temperatures...");
    sensors.requestTemperatures(); // Send the command to get temperatures
   
    // After we got the temperatures, we can print them here.
    // We use the function ByIndex, and as an example get the temperature from the first sensor only.
    temp0F = sensors.getTempFByIndex(0);
    temp0F = roundf(temp0F*10)/10; // 0.1 deg F precision
    ret = snprintf(temp0Fchar,sizeof temp0Fchar, "%.1f", temp0F);
    client.publish(heirnamet0, temp0Fchar);

    temp1F = sensors.getTempFByIndex(1);
    temp1F = roundf(temp1F*10)/10; // 0.1 deg F precision
    ret = snprintf(temp1Fchar,sizeof temp1Fchar, "%.1f", temp1F);
    client.publish(heirnamet1, temp1Fchar);

    difftemp0minus1F = temp0F - temp1F;
//    difftemp0minus1F = roundf(difftemp0minus1F *10)/10; // 0.1 deg F precision
    ret = snprintf(difftemp0minus1Fchar,sizeof difftemp0minus1Fchar, "%.1f", difftemp0minus1F);
    client.publish(heirnamedt, difftemp0minus1Fchar);
    
    // for serial print only print if there is change
    if (prevtemp0Fchar!=temp0Fchar) {
      strcpy(prevtemp0Fchar, temp0Fchar);
      temperaturesupdaterequiredflag = true;
    }
    if (prevtemp1Fchar!=temp1Fchar) {
      strcpy(prevtemp1Fchar, temp1Fchar);
      temperaturesupdaterequiredflag = true;
    }
  }

  //ultrasonic
  ultraupdaterequiredflag = false;
  if (now - ultradistancelastMsg > 100) {
//    Serial.print(" inside ultra with time of ");
//    Serial.print(ultradistancelastMsg );
    ultradistancelastMsg = now;
//    ultracountlastMsg = 0;

    digitalWrite(ultratrigPin, LOW); 
    delayMicroseconds(2); 
    digitalWrite(ultratrigPin, HIGH);
    delayMicroseconds(10); 
    digitalWrite(ultratrigPin, LOW);
    ultraduration = pulseIn(ultraechoPin, HIGH);
    ultradistance = (ultraduration/2) / 29.1;
  
    ultradistancediff = ultrapreviousdistance - ultradistance;
    if (abs(ultradistancediff)>1) {
      ultrapreviousdistance = ultradistance;
      ultraupdaterequiredflag = true;
      sprintf(ultradistancechar , "%d", ultradistance);
//      sprintf(ultradistancechar , "%04d", ultradistance);
      client.publish(heirnamedistance , ultradistancechar);
    }

    if (ultradistance <= ultracountermaxdistance){
      ultracurrentState = 1;
    }
    else {
      ultracurrentState = 0;
    }
    if(ultracurrentState != ultrapreviousState){
      if(ultracurrentState == 1){
        ultracounter = ultracounter + 1;
        if (ultracounter > ultrarollovercount ) {
          ultracounter = ultracounter - ultrarollovercount;      
        }
        ultraupdaterequiredflag = true;
        ultracounterchar;
        sprintf(ultracounterchar, "%d", ultracounter);
        client.publish(heirnamecount , ultracounterchar);
      }
    }
    ultrapreviousState = ultracurrentState;
  }


  //Serial.print if needed
  if ( ultraupdaterequiredflag | temperaturesupdaterequiredflag) {
    Serial.print("Counter: ");  
//    Serial.print(ultracounter); 
    Serial.print(  ultracounter/100 );
    Serial.print((ultracounter/10)%10);
    Serial.print(ultracounter%10);
    Serial.print(" Distance: ");
    Serial.print(ultradistance );
    Serial.print(" cm");  

    Serial.print("   Temps in deg F: Temp0: ");
    Serial.print(temp0Fchar);
    Serial.print(" and Temp1: ");
    Serial.print(temp1Fchar);
    Serial.print(" and t0-t1: ");
    Serial.print(difftemp0minus1Fchar);

    Serial.println();
  }
}
