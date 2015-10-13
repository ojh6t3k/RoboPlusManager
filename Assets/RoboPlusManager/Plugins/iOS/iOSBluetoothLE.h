//
//  iOSBluetoothLE.h
//  Unity-iPhone
//
//  Created by Tony Pitman on 03/05/2014.
//  Modified by Jaehong Oh on 10/06/2015.
//
//

#import <Foundation/Foundation.h>
#import <CoreBluetooth/CoreBluetooth.h>

typedef void (* UnityCallback)(const char* methodName, const char* arg);

extern "C" {
    void _iOSBluetoothLELogString (NSString *message);
    void _iOSBluetoothLELog (char *message);
    void _iOSBluetoothLEInitialize (BOOL asCentral, BOOL asPeripheral, UnityCallback unityCallback);
    void _iOSBluetoothLEDeInitialize ();
    void _iOSBluetoothLEPauseMessages (BOOL pause);
    void _iOSBluetoothLEScanForPeripheralsWithServices (char *serviceUUIDsStringRaw);
    void _iOSBluetoothLEStopScan ();
    void _iOSBluetoothLERetrieveListOfPeripheralsWithServices (char *serviceUUIDsStringRaw);
    void _iOSBluetoothLEConnectToPeripheral (char *name);
    void _iOSBluetoothLEDisconnectPeripheral (char *name);
    void _iOSBluetoothLEReadCharacteristic (char *name, char *service, char *characteristic);
    void _iOSBluetoothLEWriteCharacteristic (char *name, char *service, char *characteristic, unsigned char *data, int length, BOOL withResponse);
    void _iOSBluetoothLESubscribeCharacteristic (char *name, char *service, char *characteristic);
    void _iOSBluetoothLEUnSubscribeCharacteristic (char *name, char *service, char *characteristic);
    void _iOSBluetoothLEPeripheralName (char *newName);
    void _iOSBluetoothLECreateService (char *uuid, BOOL primary);
    void _iOSBluetoothLERemoveService (char *uuid);
    void _iOSBluetoothLERemoveServices ();
    void _iOSBluetoothLECreateCharacteristic (char *uuid, int properties, int permissions, unsigned char *data, int length);
    void _iOSBluetoothLERemoveCharacteristic (char *uuid);
    void _iOSBluetoothLERemoveCharacteristics ();
    void _iOSBluetoothLEStartAdvertising ();
    void _iOSBluetoothLEStopAdvertising ();
    void _iOSBluetoothLEUpdateCharacteristicValue (char *uuid, unsigned char *data, int length);
}

@interface iOSBluetoothLE : NSObject <CBCentralManagerDelegate, CBPeripheralManagerDelegate, CBPeripheralDelegate>
{
    CBCentralManager *_centralManager;
    
    NSMutableDictionary *_peripherals;
    NSMutableDictionary *_peripheralCharacteristics;
    
    CBPeripheralManager *_peripheralManager;
    
    NSString *_peripheralName;	
    
    NSMutableDictionary *_services;
    NSMutableDictionary *_characteristics;
    
    NSMutableArray *_backgroundMessages;
    BOOL _isPaused;
    BOOL _alreadyNotified;	
}

@property (atomic, strong) NSMutableDictionary *_peripherals;

- (void)initialize:(BOOL)asCentral asPeripheral:(BOOL)asPeripheral;
- (void)deInitialize;
- (void)scanForPeripheralsWithServices:(NSArray *)serviceUUIDs options:(NSDictionary *)options;
- (void)stopScan;
- (void)retrieveListOfPeripheralsWithServices:(NSArray *)serviceUUIDs;
- (void)connectToPeripheral:(NSString *)name;
- (void)disconnectPeripheral:(NSString *)name;
- (void)readCharacteristic:(NSString *)name service:(NSString *)serviceString characteristic:(NSString *)characteristicString;
- (void)writeCharacteristic:(NSString *)name service:(NSString *)serviceString characteristic:(NSString *)characteristicString data:(NSData *)data withResponse:(BOOL)withResponse;
- (void)subscribeCharacteristic:(NSString *)name service:(NSString *)serviceString characteristic:(NSString *)characteristicString;
- (void)unsubscribeCharacteristic:(NSString *)name service:(NSString *)serviceString characteristic:(NSString *)characteristicString;
- (void)peripheralName:(NSString *)newName;
- (void)createService:(NSString *)uuid primary:(BOOL)primary;
- (void)removeService:(NSString *)uuid;
- (void)removeServices;
- (void)createCharacteristic:(NSString *)uuid properties:(CBCharacteristicProperties)properties permissions:(CBAttributePermissions)permissions value:(NSData *)value;
- (void)removeCharacteristic:(NSString *)uuid;
- (void)removeCharacteristics;
- (void)startAdvertising;
- (void)stopAdvertising;
- (void)updateCharacteristicValue:(NSString *)uuid value:(NSData *)value;
- (void)pauseMessages:(BOOL)isPaused;
- (void)sendUnityMessage:(BOOL)isString message : (NSString *)message;

+ (NSString *) base64StringFromData:(NSData *)data length:(int)length;

@end

@interface UnityMessage : NSObject

{
    BOOL _isString;
    NSString *_message;
}

- (void)initialize:(BOOL)isString message:(NSString *)message;
- (void)deInitialize;
- (void)sendUnityMessage;

@end

