# MyStrom Button MQTT Relay

A simple application to relay HTTP only webhook calls from MyStrom Buttons to MQTT.

## Configuration

| Environment Variable        | Required | Description                                                                                                                   |
| ----------------------------| :------: | :---------------------------------------------------------------------------------------------------------------------------- |
| `MYSTROM_RELAY_SECRET`      |   Yes    | The relays secret can be specify to only relay calls that contain this secret, to at least make it a little bit more "secure" |
| `MQTT_SERVER`               |   Yes    | The MQTT server hostname or ip                                                                                                |
| `MQTT_SERVER_PORT`          |   Yes    | The MQTT server port                                                                                                          |
| `MQTT_SERVER_USE_TLS`       |    No    | Set to `true` if the MQTT server requires TLS, defaults to `false`                                                            |
| `MQTT_SERVER_USERNAME`      |    No    | The server credentials username                                                                                               |
| `MQTT_SERVER_PASSWORD`      |    No    | The server credentials password                                                                                               |
| `MQTT_TOPIC`                |    No    | The MQTT base topic for the button messages, defaults to `mystrom_button`                                                     |
| `HOMEASSISTANT_API_TOKEN`   |    No    | The API Token of your Home Assitant Instance                                                                                  |

## Button configuration

The MyStromRelay requires the following Button configuration for each desired action

```txt
post://[MYSTROM_RELAY_IP:PORT]/buttons?secret=[SECRET]&id=[BUTTON_ID]&action=[BUTTON_ACTION]
```

## MQTT events

The requests will be relayed to the target system `MQTT_SERVER` in the following form

### Button press

Topic:
`[MQTT_TOPIC]/[BUTTON_ACTION]`

Payload:

```txt
[BUTTON_ID]
```

### Report battery state

Topic:
`[MQTT_TOPIC]/battery`

Payload:

```txt
[BATTERY_LEVEL]
```

## Docker Compose

```yaml
mystrom-button-relay:
  image: ghcr.io/mightea/mystrom_relay
  container_name: mystrom-button-relay
  environment:
    - MQTT_SERVER=192.168.1.100
    - MQTT_SERVER_PORT=1883
    - MYSTROM_RELAY_SECRET=*****
  ports:
    - 3458:5000
```
