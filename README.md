# PoGoSlackBot

Example configuration
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
      "WalkingPoints": [ - walking points, will run in a loop
        {
          "Latitude": 40.766844,
          "Longitude": -73.979166
        },
        {
          "Latitude": 40.767169,
          "Longitude": -73.979928
        }
      ]
    }
  ]
}
```
