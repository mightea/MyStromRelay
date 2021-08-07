# MyStrom Button HTTP Relay

A simple application to relay HTTP only webhook calls from MyStrom Buttons to HTTPS only endpoints.

## Configuration

| Environment Variable       | Required | Description                                                                                                                   |
| -------------------------- | :------: | :---------------------------------------------------------------------------------------------------------------------------- |
| `MYSTROM_RELAY_TARGET_URL` |   Yes    | The target relay url, for example for Node-Red on HASS.io `https://[HOMEASSISTANT IP]:1880/endpoint/`                         |
| `MYSTROM_RELAY_SECRET`     |    No    | The relays secret can be specify to only relay calls that contain this secret, to at least make it a little bit more "secure" |

## Button configuration

The MyStromRelay requires the following Button configuration for each desired action

```
post://[MYSTROM_RELAY_IP:PORT]/buttons?secret=[SECRET]&id=[BUTTON_ID]&action=[BUTTON_ACTION]
```

## Relay target configuration

The requests will be relayed to the target system `MYSTROM_RELAY_TARGET_URL` in the following form

### Button press

Request:
`POST /buttons/trigger`

Json Body:

```json
{
  "id": "[BUTTON_ID]",
  "action": "[BUTTON_ACTION]"
}
```

### Report battery state

Request:
`POST /buttons/battery`

Json Body:

```json
{
  "mac": "[BUTTON_MAC_ADDRESS]",
  "level": "[BATTERY_LEVEL]"
}
```
