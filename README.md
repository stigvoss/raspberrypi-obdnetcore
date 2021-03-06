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

* [enable RTC drivers](https://github.com/PiSupply/PiJuice/blob/master/Software/README.md#pijuice-rtc)
* sync RTC

Restart the PiJuice service `sudo service pijuice start`

## Install .NET Core 3.0 runtime

## Setup bluetooth OBD2 adapter

* using bluetoothctl
* connect BT OBD

## Bind device and set permissions

Add the following lines to /etc/rc.local using `sudo nano /etc/rc.local`

```bash
sudo rfcomm bind /dev/rfcomm0 <macaddr> && sudo chmod 0777 /dev/rfcomm0
```

Replace <macaddr> with the MAC-address for your OBD2 adapter found using `bluetoothctl`

## Install application

* download from an url
* extract (possibly to /opt)

### Enable application on boot

* handle time before BT connected (less than 30 seconds, more than 15 seconds in test)
* add app to rc.local or make service
