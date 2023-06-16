echo %PATH%
set "PATH=%PATH%;C:\Program Files\Unity\Hub\Editor\2021.3.22f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools"
break > ./scripts/logcat.log
adb logcat -c
rem adb logcat ActivityManager:I Unity:D *:S > ./logcat.log
adb logcat Unity:D *:S > ./scripts/logcat.log
