# PoGoSlackBot

Learning project for Pokemon Go & Slack to check for spawns around the work office and check the status of "our" gym.
Requires .Net Framework 4.6

Using [POGOLib](https://github.com/AeonLucid/POGOLib) and [Slack.Webhooks](https://github.com/nerdfury/Slack.Webhooks) to interact with the services.

Can be installed as a Windows service with the [InstallUtil](https://msdn.microsoft.com/en-us/library/sd8zc8ha(v=vs.110).aspx) command.

###Printscreens

![Spawns](http://pokemon.trembon.se/slackbot1.png)

![Gyms](http://pokemon.trembon.se/slackbot2.png)


### Known bugs

- The app sometimes crashes due to invalid responses in POGOLib


###Example configuration (settings.json)
```
{
  "MapURLFormat": "http://maps.google.com/?q={0},{1}",
  "ImageURLFormat": "http://some.site.com/pokemon_images/{0}.png", - ex: http://some.site.com/pokemon_images/pidgey.png
  "Instances": [
    {
      "Name": "Name of the instance", - only to keep instances apart
      "LoginProvider": "GoogleAuth", -  or PokemonTrainerClub
      "Username": "example", - account username, works with example@gmail.com
      "Password": "superSecretPassword1!", - account password
      "ProcessNearbyPokemon": true, - if notification about nearby pokemon should be sent to slack
      "ProcessGyms": true, - if notification about gyms should be sent to slack
      "SlackWebHookURL": "https://hooks.slack.com/services/[randomletts]", - the slack webhook url to send messages to
      "SlackChannel": "#pokemongo", - name of the slack channel
      "SlackBotName": "Pokemon GO Scanner", - name of the bot in slack
      "WalkingPoints": [ - walking points, will run in a loop like 1,2,3,1,2,3
        {
          "Latitude": 40.766844,
          "Longitude": -73.979166
        },
        {
          "Latitude": 40.767169,
          "Longitude": -73.979928
        },
        {
          "Latitude": 40.767169,
          "Longitude": -73.979938
        }
      ]
    }
  ]
}
```
