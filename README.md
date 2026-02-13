<h1 align="center">Project SCRAMBLE</h1>
<div align="center">
<a href="https://github.com/MS-crew/ProjectSCRAMBLE/releases"><img src="https://img.shields.io/github/downloads/MS-crew/ProjectSCRAMBLE/total?style=for-the-badge&logo=githubactions&label=Downloads" href="https://github.com/MS-crew/ProjectSCRAMBLE/releases" alt="GitHub Release Download"></a>
<a href="https://github.com/MS-crew/ProjectSCRAMBLE/releases"><img src="https://img.shields.io/badge/Build-1.4.1-brightgreen?style=for-the-badge&logo=gitbook" href="https://github.com/MS-crew/ProjectSCRAMBLE/releases" alt="GitHub Releases"></a>
<a href="https://github.com/MS-crew/ProjectSCRAMBLE/blob/master/LICENSE.txt"><img src="https://img.shields.io/badge/Licence-GPL_3.0-blue?style=for-the-badge&logo=gitbook" href="https://github.com/MS-crew/ProjectSCRAMBLE/blob/master/LICENSE.txt" alt="General Public License v3.0"></a>
<a href="https://github.com/ExMod-Team/EXILED"><img src="https://img.shields.io/badge/Exiled-9.6.0-red?style=for-the-badge&logo=gitbook" href="https://github.com/ExMod-Team/EXILED" alt="GitHub Exiled"></a>


Project SCRAMBLE was a project conducted by the SCP Foundation that aimed to create the SCRAMBLE Visor Unit, an eyepiece that would allow for Foundation units to view SCP-096's face without worry of triggering its response. This visor utilized SCRAMBLE, a program that utilizes a microprocessor within the visor to analyze the viewing field for the facial features of SCP-096. Upon recognition, it would immediately obfuscate the image before the light reaches the human eye.
</div>

## Installation

1. Download the release file from the GitHub page [here](https://github.com/MS-crew/ProjectSCRAMBLE/releases).
2. Extract the contents into your `\AppData\Roaming\EXILED\Plugins` directory.
3. If you are using PMER version, Download the Default Schematic file from the GitHub page [here](https://github.com/MS-crew/ProjectSCRAMBLE/releases).
4. If you are using PMER version, Extract the Schematic contents into your `\AppData\Roaming\SCP Secret Laboratory\LabAPI-Beta\configs\Yourport\ProjectMER\Schematics` directory.
5. Restart your server to apply the changes.
6. Configure the plugin according to your server’s needs using the provided settings.

## Dependicies
- ProjectMER [here](https://github.com/Michal78900/ProjectMER)
- HintServiceMeow [here](https://github.com/MeowServer/HintServiceMeow) or Ruei [here](https://github.com/pawslee/Ruei)
## Feedback and Issues

This is the initial release of the plugin. We welcome any feedback, bug reports, or suggestions for improvements.

- **Report Issues:** [Issues Page](https://github.com/MS-crew/ProjectSCRAMBLE/issues)
- **Contact:** [discerrahidenetim@gmail.com](mailto:discerrahidenetim@gmail.com)

Thank you for using our plugin and helping us improve it!
## Default Config
```yml
is_enabled: true
debug: false
# Whether to remove the main 1344 effect when using SCRAMBLES
remove_orginal1344_effect: true
# If you remove the original effect, simulate the temporary darkness when wearing the glasses
simulate_temporary_darkness: true
# Whether the SCRAMBLES will use charge while blocking SCP-096 face
scramble_charge: true
# How much power should the SCRAMBLEs use to obfuscate 96's face? (1 = default, >1 = faster, <1 = slower)
charge_usage_multiplayer: 1
# Attach to head or Directl attach to player
attach_censor_to_head: true
# 0.1 is okey, 0.01 better/good , 0.001 greater
attach_to_headsync_interval: 0.01
# Censor schematic name
censor_schematic: 'Censormain'
# Censor schematic scale
censor_schematic_scale:
  x: 0.5
  y: 0.5
  z: 0.5
# Wearing time (default 5)
activate_time: 1
# Removal time (default 5.1)
deactivate_time: 1
# Custom item settings
project_s_c_r_a_m_b_l_e:
  can_wear_off: true
  id: 1730
  weight: 1
  name: 'Project SCRAMBLE'
  spawn_properties:
    limit: 0
    dynamic_spawn_points: []
    static_spawn_points: []
    role_spawn_points: []
    room_spawn_points: []
    locker_spawn_points: []
  description: 'An artificial intelligence Visor that censors SCP-096''s face'
  scale:
    x: 1
    y: 1
    z: 1
# Hint settings
hint:
  x_cordinate: 370
  y_cordinate: 90
  font_size: 20
  # 0 = left , 1 = right, 2 = center
  alligment: 0
```
