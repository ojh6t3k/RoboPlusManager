//
//  iOSBluetoothLE.h
//  Unity-iPhone
//
//  Created by Tony Pitman on 03/05/2014.
//  Modified by Jaehong Oh on 10/06/2015.
//
//

#import "iOSBluetoothLE.h"


extern "C" {
    
    iOSBluetoothLE *_iOSBluetoothLE = nil;
    UnityCallback _unityCallback = nil;
    const char *_unityMessageMethodName = "iOSBluetoothLEMessage";
    const char *_unityDataMethodName = "iOSBluetoothLEData";
    
    void _iOSBluetoothLELogString (NSString *message)
    {
        NSLog (@"%@", message);
    }
    
    void _iOSBluetoothLELog (char *message)
    {
        _iOSBluetoothLELogString ([NSString stringWithFormat:@"%s", message]);
    }

    void _iOSBluetoothLEInitialize (BOOL asCentral, BOOL asPeripheral, UnityCallback unityCallback)
    {
        _iOSBluetoothLE = [iOSBluetoothLE new];
        [_iOSBluetoothLE initialize:asCentral asPeripheral:asPeripheral];

        _unityCallback = unityCallback;

        if(_unityCallback != nil)
            _unityCallback(_unityMessageMethodName, "Initialized");
    }
    
    void _iOSBluetoothLEDeInitialize ()
    {
        if (_iOSBluetoothLE != nil) {
            
            [_iOSBluetoothLE deInitialize];
            [_iOSBluetoothLE release];
            _iOSBluetoothLE = nil;
            
            if(_unityCallback != nil)
                _unityCallback(_unityMessageMethodName, "DeInitialized");
        }
    }
    
    void _iOSBluetoothLEPauseMessages (BOOL pause)
    {
        if (_iOSBluetoothLE != nil)
            [_iOSBluetoothLE pauseMessages:pause];
    }
    
    void _iOSBluetoothLEScanForPeripheralsWithServices (char *serviceUUIDsStringRaw)
    {
        if (_iOSBluetoothLE != nil)
        {
            NSMutableArray *actualUUIDs = nil;
            
            if (serviceUUIDsStringRaw != nil)
            {
                NSString *serviceUUIDsString = [NSString stringWithFormat:@"%s", serviceUUIDsStringRaw];
                NSArray *serviceUUIDs = [serviceUUIDsString componentsSeparatedByString:@"|"];
                
                if (serviceUUIDs.count > 0)
                {
                    actualUUIDs = [[NSMutableArray alloc] init];
                    
                    for (NSString* sUUID in serviceUUIDs)
                        [actualUUIDs addObject:[CBUUID UUIDWithString:sUUID]];
                }
            }
            
            [_iOSBluetoothLE scanForPeripheralsWithServices:actualUUIDs options:nil];
        }
    }
    
    void _iOSBluetoothLEStopScan ()
    {
        if (_iOSBluetoothLE != nil)
            [_iOSBluetoothLE stopScan];
    }
    
    void _iOSBluetoothLERetrieveListOfPeripheralsWithServices (char *serviceUUIDsStringRaw)
    {
        if (_iOSBluetoothLE != nil)
        {
            NSMutableArray *actualUUIDs = nil;
            
            if (serviceUUIDsStringRaw != nil)
            {
                NSString *serviceUUIDsString = [NSString stringWithFormat:@"%s", serviceUUIDsStringRaw];
                NSArray *serviceUUIDs = [serviceUUIDsString componentsSeparatedByString:@"|"];
                
                if (serviceUUIDs.count > 0)
                {
                    actualUUIDs = [[NSMutableArray alloc] init];
                    
                    for (NSString* sUUID in serviceUUIDs)
                        [actualUUIDs addObject:[CBUUID UUIDWithString:sUUID]];
                }
            }
            
            [_iOSBluetoothLE retrieveListOfPeripheralsWithServices:actualUUIDs];
        }
    }
    
    void _iOSBluetoothLEConnectToPeripheral (char *name)
    {
        if (_iOSBluetoothLE && name != nil)
            [_iOSBluetoothLE connectToPeripheral:[NSString stringWithFormat:@"%s", name]];
    }
    
    void _iOSBluetoothLEDisconnectPeripheral (char *name)
    {
        if (_iOSBluetoothLE && name != nil)
            [_iOSBluetoothLE disconnectPeripheral:[NSString stringWithFormat:@"%s", name]];
    }
    
    void _iOSBluetoothLEReadCharacteristic (char *name, char *service, char *characteristic)
    {
        if (_iOSBluetoothLE && name != nil && service != nil && characteristic != nil)
            [_iOSBluetoothLE readCharacteristic:[NSString stringWithFormat:@"%s", name] service:[NSString stringWithFormat:@"%s", service] characteristic:[NSString stringWithFormat:@"%s", characteristic]];
    }
    
    void _iOSBluetoothLEWriteCharacteristic (char *name, char *service, char *characteristic, unsigned char *data, int length, BOOL withResponse)
    {
        if (_iOSBluetoothLE && name != nil && service != nil && characteristic != nil && data != nil && length > 0)
            [_iOSBluetoothLE writeCharacteristic:[NSString stringWithFormat:@"%s", name] service:[NSString stringWithFormat:@"%s", service] characteristic:[NSString stringWithFormat:@"%s", characteristic] data:[NSData dataWithBytes:data length:length] withResponse:withResponse];
    }
    
    void _iOSBluetoothLESubscribeCharacteristic (char *name, char *service, char *characteristic)
    {
        if (_iOSBluetoothLE && name != nil && service != nil && characteristic != nil)
            [_iOSBluetoothLE subscribeCharacteristic:[NSString stringWithFormat:@"%s", name] service:[NSString stringWithFormat:@"%s", service] characteristic:[NSString stringWithFormat:@"%s", characteristic]];
    }
    
    void _iOSBluetoothLEUnSubscribeCharacteristic (char *name, char *service, char *characteristic)
    {
        if (_iOSBluetoothLE && name != nil && service != nil && characteristic != nil)
            [_iOSBluetoothLE unsubscribeCharacteristic:[NSString stringWithFormat:@"%s", name] service:[NSString stringWithFormat:@"%s", service] characteristic:[NSString stringWithFormat:@"%s", characteristic]];
    }
    
    void _iOSBluetoothLEPeripheralName (char *newName)
    {
        if (_iOSBluetoothLE != nil && newName != nil)
            [_iOSBluetoothLE peripheralName:[[NSString alloc] initWithUTF8String:newName]];
    }
    
    void _iOSBluetoothLECreateService (char *uuid, BOOL primary)
    {
        if (_iOSBluetoothLE != nil)
            [_iOSBluetoothLE createService:[NSString stringWithFormat:@"%s", uuid] primary:primary];
    }
    
    void _iOSBluetoothLERemoveService (char *uuid)
    {
        if (_iOSBluetoothLE != nil)
            [_iOSBluetoothLE removeService:[NSString stringWithFormat:@"%s", uuid]];
    }
    
    void _iOSBluetoothLERemoveServices ()
    {
        if (_iOSBluetoothLE != nil)
            [_iOSBluetoothLE removeServices];
    }
    
    void _iOSBluetoothLECreateCharacteristic (char *uuid, int properties, int permissions, unsigned char *data, int length)
    {
        if (_iOSBluetoothLE != nil)
        {
            NSData *value = nil;
            if (data != nil)
                value = [[NSData alloc] initWithBytes:data length:length];
            
            [_iOSBluetoothLE createCharacteristic:[NSString stringWithFormat:@"%s", uuid] properties:properties permissions:permissions value:value];
        }
    }
    
    void _iOSBluetoothLERemoveCharacteristic (char *uuid)
    {
        if (_iOSBluetoothLE != nil)
            [_iOSBluetoothLE removeCharacteristic:[NSString stringWithFormat:@"%s", uuid]];
    }
    
    void _iOSBluetoothLERemoveCharacteristics ()
    {
        if (_iOSBluetoothLE != nil)
            [_iOSBluetoothLE removeCharacteristics];
    }
    
    void _iOSBluetoothLEStartAdvertising ()
    {
        if (_iOSBluetoothLE != nil)
            [_iOSBluetoothLE startAdvertising];
    }
    
    void _iOSBluetoothLEStopAdvertising ()
    {
        if (_iOSBluetoothLE != nil)
            [_iOSBluetoothLE stopAdvertising];
    }
    
    void _iOSBluetoothLEUpdateCharacteristicValue (char *uuid, unsigned char *data, int length)
    {
        if (_iOSBluetoothLE != nil)
        {
            NSData *value = nil;
            if (data != nil)
                value = [[NSData alloc] initWithBytes:data length:length];
            
            [_iOSBluetoothLE updateCharacteristicValue:[NSString stringWithFormat:@"%s", uuid] value:value];
        }
    }
}

@implementation iOSBluetoothLE

@synthesize _peripherals;


- (void)initialize:(BOOL)asCentral asPeripheral:(BOOL)asPeripheral
{
    _isPaused = FALSE;
    
    _centralManager = nil;
    _peripheralManager = nil;
    _services = nil;
    _characteristics = nil;
    
    if (asCentral)
        _centralManager = [[CBCentralManager alloc] initWithDelegate:self queue:nil];
    
    if (asPeripheral)
        _peripheralManager = [[CBPeripheralManager alloc] initWithDelegate:self queue:nil];
    
    _services = [[NSMutableDictionary alloc] init];
    _characteristics = [[NSMutableDictionary alloc] init];
    _peripherals = [[NSMutableDictionary alloc] init];
    _peripheralCharacteristics = [[NSMutableDictionary alloc] init];
}

- (void)deInitialize
{
    if (_backgroundMessages != nil)
    {
        for (UnityMessage *message in _backgroundMessages)
        {
            if (message != nil)
            {
                [message deInitialize];
                [message release];
            }
        }
        
        [_backgroundMessages release];
        _backgroundMessages = nil;
        
        if (_peripheralManager != nil)
            [self stopAdvertising];
        
        [self removeCharacteristics];
        [self removeServices];
        
        if (_centralManager != nil)
            [self stopScan];
        
        [_peripherals removeAllObjects];
        [_peripheralCharacteristics removeAllObjects];
    }
}

- (void)pauseMessages:(BOOL)isPaused
{
    if (isPaused != _isPaused)
    {
        if (_backgroundMessages == nil)
            _backgroundMessages = [[NSMutableArray alloc] init];
        
        _isPaused = isPaused;
        
        // if we are not paused now since we know we changed state
        // that means we were paused so we need to pump the saved
        // messages to Unity
        if (isPaused) {
            
            if (_backgroundMessages != nil)
            {
                for (UnityMessage *message in _backgroundMessages)
                {
                    if (message != nil)
                    {                        
                        [message sendUnityMessage];
                        [message deInitialize];
                        [message release];
                    }
                }
                
                [_backgroundMessages removeAllObjects];
            }
        }
    }
}

- (void)createService:(NSString *)uuid primary:(BOOL)primary
{
    CBUUID *cbuuid = [CBUUID UUIDWithString:uuid];
    CBMutableService *service = [[CBMutableService alloc] initWithType:cbuuid primary:primary];
    
    NSMutableArray *characteristics = [[NSMutableArray alloc] init];
    
    NSEnumerator *enumerator = [_characteristics keyEnumerator];
    id key;
    while ((key = [enumerator nextObject]))
        [characteristics addObject:[_characteristics objectForKey:key]];
    
    service.characteristics = characteristics;
    
    [_services setObject:service forKey:cbuuid];
    
    if (_peripheralManager != nil)
    {
        [_peripheralManager addService:service];
    }
}

- (void)removeService:(NSString *)uuid
{
    if (_services != nil)
    {
        if (_peripheralManager != nil)
        {
            CBMutableService *service = [_services objectForKey:uuid];
            if (service != nil)
                [_peripheralManager removeService:service];
        }
        
        [_services removeObjectForKey:uuid];
    }
}

- (void)removeServices
{
    if (_services != nil)
    {
        [_services removeAllObjects];
        
        if (_peripheralManager != nil)
            [_peripheralManager removeAllServices];
    }
}

- (void)peripheralName:(NSString *)newName
{
    _peripheralName = newName;
}

- (void)createCharacteristic:(NSString *)uuid properties:(CBCharacteristicProperties)properties permissions:(CBAttributePermissions)permissions value:(NSData *)value
{
    CBUUID *cbuuid = [CBUUID UUIDWithString:uuid];
    CBCharacteristic *characteristic = [[CBMutableCharacteristic alloc] initWithType:cbuuid properties:properties value:value permissions:permissions];
    
    [_characteristics setObject:characteristic forKey:cbuuid];
}

- (void)removeCharacteristic:(NSString *)uuid
{
    if (_characteristics != nil)
        [_characteristics removeObjectForKey:uuid];
}

- (void)removeCharacteristics
{
    if (_characteristics != nil)
        [_characteristics removeAllObjects];
}

- (void)startAdvertising
{
    if (_peripheralManager != nil && _services != nil)
    {
        NSMutableArray *services = [[NSMutableArray alloc] init];
        
        NSEnumerator *enumerator = [_services keyEnumerator];
        id key;
        while ((key = [enumerator nextObject]))
        {
            CBMutableService *service = [_services objectForKey:key];
            [services addObject:service.UUID];
        }
        
        if (_peripheralName == nil)
            _peripheralName = @"";
        
        [_peripheralManager startAdvertising:@{ CBAdvertisementDataServiceUUIDsKey : services, CBAdvertisementDataLocalNameKey : _peripheralName }];
    }
}

- (void)stopAdvertising
{
    if (_peripheralManager != nil)
        [_peripheralManager stopAdvertising];
}

- (void)updateCharacteristicValue:(NSString *)uuid value:(NSData *)value
{
    if (_characteristics != nil)
    {
        CBUUID *cbuuid = [CBUUID UUIDWithString:uuid];
        CBMutableCharacteristic *characteristic = [_characteristics objectForKey:cbuuid];
        if (characteristic != nil)
        {
            characteristic.value = value;
            if (_peripheralManager != nil)
                [_peripheralManager updateValue:value forCharacteristic:characteristic onSubscribedCentrals:nil];
        }
    }
}

// central delegate implementation
- (void)scanForPeripheralsWithServices:(NSArray *)serviceUUIDs options:(NSDictionary *)options
{
    if (_centralManager != nil)
    {
        if (_peripherals != nil)
            [_peripherals removeAllObjects];
        
        [_centralManager scanForPeripheralsWithServices:serviceUUIDs options:options];
    }
}

- (void) stopScan
{
    if (_centralManager != nil)
        [_centralManager stopScan];
}

- (void)retrieveListOfPeripheralsWithServices:(NSArray *)serviceUUIDs
{
    if (_centralManager != nil)
    {
        if (_peripherals != nil)
            [_peripherals removeAllObjects];
        
        NSArray * list = [_centralManager retrieveConnectedPeripheralsWithServices:serviceUUIDs];
        if (list != nil)
        {
            for (int i = 0; i < list.count; ++i)
            {
                CBPeripheral *peripheral = [list objectAtIndex:i];
                if (peripheral != nil)
                {
                    NSString *identifier = [[peripheral identifier] UUIDString];
                    NSString *name = [peripheral name];
                    
                    NSString *message = [NSString stringWithFormat:@"RetrievedConnectedPeripheral~%@~%@", identifier, name];
                    
                    if(_unityCallback != nil)
                        _unityCallback(_unityMessageMethodName, [message UTF8String]);
                    
                    [_peripherals setObject:peripheral forKey:identifier];
                }
            }
        }
    }
}

- (void)connectToPeripheral:(NSString *)name
{
    if (_peripherals != nil && name != nil)
    {
        CBPeripheral *peripheral = [_peripherals objectForKey:name];
        if (peripheral != nil)
            [_centralManager connectPeripheral:peripheral options:nil];
    }
}

- (void)disconnectPeripheral:(NSString *)name
{
    if (_peripherals != nil && name != nil)
    {
        CBPeripheral *peripheral = [_peripherals objectForKey:name];
        if (peripheral != nil)
        {
            for (int serviceIndex = 0; serviceIndex < peripheral.services.count; ++serviceIndex)
            {
                CBService *service = [peripheral.services objectAtIndex:serviceIndex];
                if (service != nil)
                {
                    for (int characteristicIndex = 0; characteristicIndex < service.characteristics.count; ++characteristicIndex)
                    {
                        CBCharacteristic *characteristic = [service.characteristics objectAtIndex:characteristicIndex];
                        if (characteristic != nil)
                        {
                            NSEnumerator *enumerator = [_peripheralCharacteristics keyEnumerator];
                            id key;
                            while ((key = [enumerator nextObject]))
                            {
                                CBMutableCharacteristic *tempCharacteristic = [_peripheralCharacteristics objectForKey:key];
                                if (tempCharacteristic != nil)
                                {
                                    if ([tempCharacteristic isEqual:characteristic])
                                    {
                                        [_peripheralCharacteristics removeObjectForKey:key];
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            [_centralManager cancelPeripheralConnection:peripheral];
        }
    }
}

- (void)readCharacteristic:(NSString *)name service:(NSString *)serviceString characteristic:(NSString *)characteristicString
{
    if (name != nil && serviceString != nil && characteristicString != nil && _peripherals != nil)
    {
        CBPeripheral *peripheral = [_peripherals objectForKey:name];
        if (peripheral != nil)
        {
            CBUUID *cbuuid = [CBUUID UUIDWithString:characteristicString];
            CBCharacteristic *characteristic = [_peripheralCharacteristics objectForKey:cbuuid];
            if (characteristic != nil)
                [peripheral readValueForCharacteristic:characteristic];
        }
    }
}

- (void)writeCharacteristic:(NSString *)name service:(NSString *)serviceString characteristic:(NSString *)characteristicString data:(NSData *)data withResponse:(BOOL)withResponse
{
    if (name != nil && serviceString != nil && characteristicString != nil && _peripherals != nil && data != nil)
    {
        CBPeripheral *peripheral = [_peripherals objectForKey:name];
        if (peripheral != nil)
        {
            CBUUID *cbuuid = [CBUUID UUIDWithString:characteristicString];
            CBCharacteristic *characteristic = [_peripheralCharacteristics objectForKey:cbuuid];
            if (characteristic != nil)
            {
                CBCharacteristicWriteType type = CBCharacteristicWriteWithoutResponse;
                if (withResponse)
                    type = CBCharacteristicWriteWithResponse;
                
                [peripheral writeValue:data forCharacteristic:characteristic type:type];
            }
        }
    }
}

- (void)subscribeCharacteristic:(NSString *)name service:(NSString *)serviceString characteristic:(NSString *)characteristicString
{
    if (name != nil && serviceString != nil && characteristicString != nil && _peripherals != nil)
    {
        CBPeripheral *peripheral = [_peripherals objectForKey:name];
        if (peripheral != nil)
        {
            CBUUID *cbuuid = [CBUUID UUIDWithString:characteristicString];
            CBCharacteristic *characteristic = [_peripheralCharacteristics objectForKey:cbuuid];
            if (characteristic != nil)
                [peripheral setNotifyValue:YES forCharacteristic:characteristic];
        }
    }
}

- (void)unsubscribeCharacteristic:(NSString *)name service:(NSString *)serviceString characteristic:(NSString *)characteristicString
{
    if (name != nil && serviceString != nil && characteristicString != nil && _peripherals != nil)
    {
        CBPeripheral *peripheral = [_peripherals objectForKey:name];
        if (peripheral != nil)
        {
            CBUUID *cbuuid = [CBUUID UUIDWithString:characteristicString];
            CBCharacteristic *characteristic = [_peripheralCharacteristics objectForKey:cbuuid];
            if (characteristic != nil)
                [peripheral setNotifyValue:NO forCharacteristic:characteristic];
        }
    }
}

- (void)centralManagerDidUpdateState:(CBCentralManager *)central
{
    _iOSBluetoothLELogString ([NSString stringWithFormat:@"Central State Update: %d", (int)central.state]);
	
	if(((int)central.state) == 5)
    {
        if(_unityCallback != nil)
            _unityCallback(_unityMessageMethodName, "BLESupported");
    }
	else
    {
        if(_unityCallback != nil)
            _unityCallback(_unityMessageMethodName, "BLENotSupported");
    }
}

- (void)centralManager:(CBCentralManager *)central didRetrievePeripherals:(NSArray *)peripherals
{
    
}

- (void)centralManager:(CBCentralManager *)central didRetrieveConnectedPeripherals:(NSArray *)peripherals
{
    
}

- (void)centralManager:(CBCentralManager *)central didFailToConnectPeripheral:(CBPeripheral *)peripheral error:(NSError *)error
{
    if (error)
    {
        NSString *message = [NSString stringWithFormat:@"Error~%@", error.description];
        
        if(_unityCallback != nil)
            _unityCallback(_unityMessageMethodName, [message UTF8String]);
    }
}

- (void)centralManager:(CBCentralManager *)central didDiscoverPeripheral:(CBPeripheral *)peripheral advertisementData:(NSDictionary *)advertisementData RSSI:(NSNumber *)RSSI
{
    NSString *name = [advertisementData objectForKey:CBAdvertisementDataLocalNameKey];
    if (_peripherals != nil && peripheral != nil && name != nil)
    {
        NSString *identifier = nil;
        
        NSString *foundPeripheral = [peripheral.identifier UUIDString]; //[self findPeripheralName:peripheral];
        if (foundPeripheral == nil)
            identifier = [[NSUUID UUID] UUIDString];
        else
            identifier = foundPeripheral;
        
        // make mac address
        NSString *manufactureData = [NSString stringWithFormat:@"%@", [advertisementData objectForKey:@"kCBAdvDataManufacturerData"]];
        manufactureData = [manufactureData stringByReplacingOccurrencesOfString:@" " withString:@""];
        
		if (manufactureData.length == 20)
		{
			NSString *mac = [manufactureData substringWithRange:NSMakeRange(5, 12)];
			mac = [NSString stringWithFormat:@"%@-%@-%@-%@-%@-%@",
				   [mac substringWithRange:NSMakeRange(0, 2)],
				   [mac substringWithRange:NSMakeRange(2, 2)],
				   [mac substringWithRange:NSMakeRange(4, 2)],
				   [mac substringWithRange:NSMakeRange(6, 2)],
				   [mac substringWithRange:NSMakeRange(8, 2)],
				   [mac substringWithRange:NSMakeRange(10, 2)]];
        
			NSString *message = [NSString stringWithFormat:@"DiscoveredPeripheral~%@~%@~%@", identifier, name, mac];

			[_peripherals setObject:peripheral forKey:identifier];

            if(_unityCallback != nil)
                _unityCallback(_unityMessageMethodName, [message UTF8String]);
		}
    }
}

- (void)centralManager:(CBCentralManager *)central didDisconnectPeripheral:(CBPeripheral *)peripheral error:(NSError *)error
{
    if (_peripherals != nil)
    {
        NSString *foundPeripheral = [self findPeripheralName:peripheral];
        if (foundPeripheral != nil)
        {
            NSString *message = [NSString stringWithFormat:@"DisconnectedPeripheral~%@", foundPeripheral];
            
            if(_unityCallback != nil)
                _unityCallback(_unityMessageMethodName, [message UTF8String]);
        }
    }
}

- (void)centralManager:(CBCentralManager *)central didConnectPeripheral:(CBPeripheral *)peripheral
{
    NSString *foundPeripheral = [self findPeripheralName:peripheral];
    if (foundPeripheral != nil)
    {
        NSString *message = [NSString stringWithFormat:@"ConnectedPeripheral~%@", foundPeripheral];
        if(_unityCallback != nil)
            _unityCallback(_unityMessageMethodName, [message UTF8String]);
        
        peripheral.delegate = self;
        
        [peripheral discoverServices:nil];
    }
}

- (CBPeripheral *) findPeripheralInList:(CBPeripheral*)peripheral
{
    CBPeripheral *foundPeripheral = nil;
    
    NSEnumerator *enumerator = [_peripherals keyEnumerator];
    id key;
    while ((key = [enumerator nextObject]))
    {
        CBPeripheral *tempPeripheral = [_peripherals objectForKey:key];
        if ([tempPeripheral isEqual:peripheral])
        {
            foundPeripheral = tempPeripheral;
            break;
        }
    }
    
    return foundPeripheral;
}

- (NSString *) findPeripheralName:(CBPeripheral*)peripheral
{
    NSString *foundPeripheral = nil;
    
    NSEnumerator *enumerator = [_peripherals keyEnumerator];
    id key;
    while ((key = [enumerator nextObject]))
    {
        CBPeripheral *tempPeripheral = [_peripherals objectForKey:key];
        if ([tempPeripheral isEqual:peripheral])
        {
            foundPeripheral = key;
            break;
        }
    }
    
    return foundPeripheral;
}

// peripheral delegate implementation
- (void)peripheral:(CBPeripheral *)peripheral didDiscoverServices:(NSError *)error
{
    if (error)
    {
        NSString *message = [NSString stringWithFormat:@"Error~%@", error.description];
        if(_unityCallback != nil)
            _unityCallback(_unityMessageMethodName, [message UTF8String]);
    }
    else
    {
        NSString *foundPeripheral = [self findPeripheralName:peripheral];
        if (foundPeripheral != nil)
        {
            for (CBService *service in peripheral.services)
            {
                NSString *message = [NSString stringWithFormat:@"DiscoveredService~%@~%@", foundPeripheral, [service UUID]];
                if(_unityCallback != nil)
                    _unityCallback(_unityMessageMethodName, [message UTF8String]);
                
                [peripheral discoverCharacteristics:nil forService:service];
            }
        }
    }
}

- (void)peripheral:(CBPeripheral *)peripheral didDiscoverCharacteristicsForService:(CBService *)service error:(NSError *)error
{
    if (error)
    {
        NSString *message = [NSString stringWithFormat:@"Error~%@", error.description];
        if(_unityCallback != nil)
            _unityCallback(_unityMessageMethodName, [message UTF8String]);
    }
    else
    {
        NSString *foundPeripheral = [self findPeripheralName:peripheral];
        if (foundPeripheral != nil)
        {
            for (CBCharacteristic *characteristic in service.characteristics)
            {
                NSString *message = [NSString stringWithFormat:@"DiscoveredCharacteristic~%@~%@~%@", foundPeripheral, [service UUID], [characteristic UUID]];
                if(_unityCallback != nil)
                    _unityCallback(_unityMessageMethodName, [message UTF8String]);
                
                [_peripheralCharacteristics setObject:characteristic forKey:[characteristic UUID]];
            }
        }
    }
}

- (void)peripheral:(CBPeripheral *)peripheral didUpdateValueForCharacteristic:(CBCharacteristic *)characteristic error:(NSError *)error
{
    if (error)
    {
        NSString *message = [NSString stringWithFormat:@"Error~%@", error.description];
        if(_unityCallback != nil)
            _unityCallback(_unityMessageMethodName, [message UTF8String]);
    }
    else
    {
        if (characteristic.value != nil)
        {
            NSString *message = [NSString stringWithFormat:@"DidUpdateValueForCharacteristic~%@~%@", [characteristic UUID], [iOSBluetoothLE base64StringFromData:characteristic.value length:(int)characteristic.value.length]];
            if(_unityCallback != nil)
                _unityCallback(_unityMessageMethodName, [message UTF8String]);

            //NSString *message = [UnityBluetoothLE base64StringFromData:characteristic.value length:characteristic.value.length];
            //UnitySendMessage (_unityObject, _unityData, [message UTF8String] );
        }
    }
}

- (void)peripheral:(CBPeripheral *)peripheral didWriteValueForCharacteristic:(CBCharacteristic *)characteristic error:(NSError *)error
{
    if (error)
    {
        NSString *message = [NSString stringWithFormat:@"Error~%@", error.description];
        
        if(_unityCallback != nil)
            _unityCallback(_unityMessageMethodName, [message UTF8String]);
    }
    else
    {
        NSString *message = [NSString stringWithFormat:@"DidWriteCharacteristic~%@", characteristic.UUID];
        
        if(_unityCallback != nil)
            _unityCallback(_unityMessageMethodName, [message UTF8String]);
    }
}

- (void)peripheral:(CBPeripheral *)peripheral didUpdateNotificationStateForCharacteristic:(CBCharacteristic *)characteristic error:(NSError *)error
{
    if (error)
    {
        NSString *message = [NSString stringWithFormat:@"Error~%@", error.description];
        
        if(_unityCallback != nil)
            _unityCallback(_unityMessageMethodName, [message UTF8String]);
    }
    else
    {
        NSString *message = [NSString stringWithFormat:@"DidUpdateNotificationStateForCharacteristic~%@", characteristic.UUID];
        
        if(_unityCallback != nil)
            _unityCallback(_unityMessageMethodName, [message UTF8String]);
    }
}

// peripheral manager delegate implementation
- (void)peripheralManagerDidUpdateState:(CBPeripheralManager *)peripheral
{
    _iOSBluetoothLELogString ([NSString stringWithFormat:@"Peripheral State Update: %d", (int)peripheral.state]);
}

- (void)peripheralManager:(CBPeripheralManager *)peripheral didAddService:(CBService *)service error:(NSError *)error
{
    if (error)
    {
        NSString *message = [NSString stringWithFormat:@"Error~%@", error.description];
        
        if(_unityCallback != nil)
            _unityCallback(_unityMessageMethodName, [message UTF8String]);
    }
    else
    {
        NSString *message = [NSString stringWithFormat:@"ServiceAdded~%@", service.UUID];
        
        if(_unityCallback != nil)
            _unityCallback(_unityMessageMethodName, [message UTF8String]);
    }
}

- (void)peripheralManagerDidStartAdvertising:(CBPeripheralManager *)peripheral error:(NSError *)error
{
    if (error)
    {
        NSString *message = [NSString stringWithFormat:@"Error~%@", error.description];
        
        if(_unityCallback != nil)
            _unityCallback(_unityMessageMethodName, [message UTF8String]);
    }
    else
    {
        NSString *message = [NSString stringWithFormat:@"StartedAdvertising"];
        
        if(_unityCallback != nil)
            _unityCallback(_unityMessageMethodName, [message UTF8String]);
    }
}

- (void)peripheralManager:(CBPeripheralManager *)peripheral central:(CBCentral *)central didSubscribeToCharacteristic:(CBCharacteristic *)characteristic
{
    
}

- (void)peripheralManager:(CBPeripheralManager *)peripheral central:(CBCentral *)central didUnsubscribeFromCharacteristic:(CBCharacteristic *)characteristic
{
    
}

- (void)peripheralManager:(CBPeripheralManager *)peripheral didReceiveReadRequest:(CBATTRequest *)request
{
    BOOL success = FALSE;
    
    if (_peripheralManager != nil)
    {
        CBMutableCharacteristic *characteristic = [_characteristics objectForKey:request.characteristic.UUID];
        
        if (characteristic != nil)
        {
            request.value = [characteristic.value subdataWithRange:NSMakeRange(request.offset, characteristic.value.length - request.offset)];
            [_peripheralManager respondToRequest:request withResult:CBATTErrorSuccess];
            
            success = TRUE;
        }
    }
    
    if (!success)
        [_peripheralManager respondToRequest:request withResult:CBATTErrorAttributeNotFound];
}

- (void)peripheralManager:(CBPeripheralManager *)peripheral didReceiveWriteRequests:(NSArray *)requests
{
    BOOL success = FALSE;
    
    if (_peripheralManager != nil)
    {
        for (int i = 0; i < requests.count; ++i)
        {
            CBATTRequest *request = [requests objectAtIndex:i];
            if (request != nil)
            {
                CBMutableCharacteristic *characteristic = [_characteristics objectForKey:request.characteristic.UUID];
                
                if (characteristic != nil)
                {
                    characteristic.value = request.value;
                    success = TRUE;
                }
                else
                {
                    success = FALSE;
                    break;
                }
            }
            else
            {
                success = FALSE;
                break;
            }
        }
    }
    
    if (success)
        [_peripheralManager respondToRequest:[requests objectAtIndex:0] withResult:CBATTErrorSuccess];
    else
        [_peripheralManager respondToRequest:[requests objectAtIndex:0] withResult:CBATTErrorAttributeNotFound];
}

- (void)sendUnityMessage:(BOOL)isString message:(NSString *)message
{
    if (_isPaused) {
        
        if (_backgroundMessages != nil) {
            
            UnityMessage *unitymessage = [[UnityMessage alloc] init];
            if (unitymessage != nil) {
                
                [unitymessage initialize:isString message:message];
                [_backgroundMessages addObject:unitymessage];
            }
        }
    }
    else {
        
        if (isString)
        {
            if(_unityCallback != nil)
                _unityCallback(_unityMessageMethodName, [message UTF8String]);
        }
        else
        {
            if(_unityCallback != nil)
                _unityCallback(_unityDataMethodName, [message UTF8String]);
        }
    }
}

static char base64EncodingTable[64] =
{
    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
    'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f',
    'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
    'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/'
};

+ (NSString *) base64StringFromData: (NSData *)data length: (int)length
{
    unsigned long ixtext, lentext;
    long ctremaining;
    unsigned char input[3], output[4];
    short i, charsonline = 0, ctcopy;
    const unsigned char *raw;
    NSMutableString *result;
    
    lentext = [data length];
    if (lentext < 1)
        return @"";
    result = [NSMutableString stringWithCapacity: lentext];
    raw = (const unsigned char *)[data bytes];
    ixtext = 0;
    
    while (true) {
        ctremaining = lentext - ixtext;
        if (ctremaining <= 0)
            break;
        for (i = 0; i < 3; i++) {
            unsigned long ix = ixtext + i;
            if (ix < lentext)
                input[i] = raw[ix];
            else
                input[i] = 0;
        }
        output[0] = (input[0] & 0xFC) >> 2;
        output[1] = ((input[0] & 0x03) << 4) | ((input[1] & 0xF0) >> 4);
        output[2] = ((input[1] & 0x0F) << 2) | ((input[2] & 0xC0) >> 6);
        output[3] = input[2] & 0x3F;
        ctcopy = 4;
        switch (ctremaining) {
            case 1:
                ctcopy = 2;
                break;
            case 2:
                ctcopy = 3;
                break;
        }
        
        for (i = 0; i < ctcopy; i++)
            [result appendString: [NSString stringWithFormat: @"%c", base64EncodingTable[output[i]]]];
        
        for (i = ctcopy; i < 4; i++)
            [result appendString: @"="];
        
        ixtext += 3;
        charsonline += 4;
        
        if ((length > 0) && (charsonline >= length))
            charsonline = 0;
    }
    return result;
}

#pragma mark Internal

@end

@implementation UnityMessage

- (void)initialize:(BOOL)isString message:(NSString *)message
{
    _isString = isString;
    _message = [message copy];
}

- (void)deInitialize
{
    if (_message != nil)
        [_message release];
    _message = nil;
}

- (void)sendUnityMessage
{
    if (_message != nil) {
        
        if (_isString)
        {
            if(_unityCallback != nil)
                _unityCallback(_unityMessageMethodName, [_message UTF8String]);
        }
        else
        {
            if(_unityCallback != nil)
                _unityCallback(_unityDataMethodName, [_message UTF8String]);
        }
    }
}

@end
