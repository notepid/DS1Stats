# DS1Stats
Program that monitors a Dark Souls 1 save game and extracts info into text files and json. 
This program is very beta and no support will be given.

Offsets etc was found here: http://dsfp.readthedocs.io/datasheet.html

File | What
--- | --- 
`ds1counter_{saveslot}_deaths.txt`					|	Total number of deaths for this character
`ds1counter_{saveslot}_name.txt`						|	The name of this character
`ds1counter_{saveslot}_time_since_last_death.txt`		|	hour:minute:seconds since last death
`ds1counter_{saveslot}_session_deaths.txt`			|	Number of deaths since this program was started (restart program for each session)
`ds1counter_{saveslot}_session_deaths_total_deaths.txt`			|	Displayed as: session deaths / total deaths
`ds1counter_{saveslot}_timespan_last_death.txt`			|	Time since last death displayed as "hh:mm:ss" 
`ds1counter_{saveslot}_average_life_length.txt`			|	Calculated average life length (session playtime divided by number of deaths)
`ds1counter_{saveslot}_stats_*.txt`			|	Various character stats like Strength, Dexterity, etc
`ds1counter.json`										|	All above stats as a JSON formatted file


If you like and use this program for your stream, or whatever, it would be nice if you included some creds to Notepid :)

# Known issues
* There MIGHT be a problem when running a save with multiple characters in it. Still being tested. Best results if used with the first saveslot.
* Only works with the Steam version of the game (Prepeare to Die edition). Can be tweaked for old versions of the game, but offsets needs to be adjusted in SaveGameOffsets.cs for that to work.