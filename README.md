# Raspberry Pi OBD logger
Project for logging OBD data on a Raspberry Pi 3 using Raspbian, .NET Core 3.0 and a PiJuice

## Raspbian

* Use Raspbian Lite

## Install and configure the PiJuice

```bash
sudo apt install pijuice-base
```

Open PiJuice configuration file using: `sudo nano /var/lib/pijuice/pijuice_config.JSON`

* shutdown on no_power event
* start on power connected

```json
{
        "system_events": {
                "no_power": {
                        "function": "SYS_FUNC_HALT_POW_OFF",
                        "enabled": true
                }
        },
        "system_task": {
                "enabled": true,
                "wakeup_on_charge": {
                        "enabled": true,
                        "trigger_level": "0"
                }
        }
}
```

## Install .NET Core 3.0 runtime

## Setup bluetooth OBD2 adapter

* using bluetoothctl

## Bind device and set permissions

Add the following lines to /etc/rc.local using `sudo nano /etc/rc.local`

```bash
sudo rfcomm bind /dev/rfcomm0 <macaddr> && sudo chmod 0777 /dev/rfcomm0
```

Replace <macaddr> with the MAC-address for your OBD2 adapter found using `bluetoothctl`
