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
3. If you are using PMER version, Download the Default Schematic file from the GitHub page [here](https://release-assets.githubusercontent.com/github-production-release-asset/982096733/d58f8329-5b31-44c1-b0ff-cc646b95abd0?sp=r&sv=2018-11-09&sr=b&spr=https&se=2026-02-13T11%3A46%3A08Z&rscd=attachment%3B+filename%3DCensorSchematicForPmerVersions.rar&rsct=application%2Foctet-stream&skoid=96c2d410-5711-43a1-aedd-ab1947aa7ab0&sktid=398a6654-997b-47e9-b12b-9515b896b4de&skt=2026-02-13T10%3A45%3A34Z&ske=2026-02-13T11%3A46%3A08Z&sks=b&skv=2018-11-09&sig=m2RwDb6Qmi9ou6B6z7DZh6IcQ6y4HM%2BheRRY6NFBItc%3D&jwt=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmVsZWFzZS1hc3NldHMuZ2l0aHVidXNlcmNvbnRlbnQuY29tIiwia2V5Ijoia2V5MSIsImV4cCI6MTc3MDk3OTgzNCwibmJmIjoxNzcwOTc5NTM0LCJwYXRoIjoicmVsZWFzZWFzc2V0cHJvZHVjdGlvbi5ibG9iLmNvcmUud2luZG93cy5uZXQifQ.zEbrPSTCDYfpR0NI4AlRGw1vf5nDUhYofSnch7-3bMs&response-content-disposition=attachment%3B%20filename%3DCensorSchematicForPmerVersions.rar&response-content-type=application%2Foctet-stream).
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
## Default Config FOR RUEI VERSION
```yml
is_enabled: true
debug: false
# Should there be a Random error in the artificial intelligence of the glasses?
random_error: false
# Random error chance
random_error_chance: 0.00100000005
# Whether the SCRAMBLES will use charge while blocking SCP-096 face
scramble_charge: true
# How much power should the SCRAMBLEs use to obfuscate 96's face? (1 = default, >1 = faster, <1 = slower)
charge_usage_multiplayer: 1
# Attach to head or Directly attach to player
attach_censor_to_head: true
# 0.1 is okey, 0.01 better/good , 0.001 greater
attach_to_headsync_interval: 0.00999999978
# Censor type as primitive
censor_type: Cube
# Rotate censor randomly
censor_rotate: true
# Censor Color
censor_color:
  r: 0
  g: 0
  b: 0
  a: 1
# Censor scale
censor_scale:
  x: 0.5
  y: 0.5
  z: 0.5
# Custom item settings
project_s_c_r_a_m_b_l_e:
  id: 1730
  weight: 1
  wearing_time: 1
  removing_time: 1
  name: 'Project SCRAMBLE'
  spawn_properties:
    limit: 0
    dynamic_spawn_points: []
    static_spawn_points: []
    role_spawn_points: []
    room_spawn_points: []
    locker_spawn_points: []
  description: 'An artificial intelligence Visor that censors SCP-096''s face'
  type: SCP1344
  remove1344_effect: true
  can_be_remove_safely: true
  scale:
    x: 1
    y: 1
    z: 1
# Hint settings
hint:
  y_cordinate: 90
```
