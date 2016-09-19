# DS1Stats
Program that monitors a Dark Souls 1 save game and extracts info into text files and json

File | What
--- | --- 
`ds1counter_{saveslot}_deaths.txt`					|	Total number of deaths for this character
`ds1counter_{saveslot}_name.txt`						|	The name of this character
`ds1counter_{saveslot}_time_since_last_death.txt`		|	hour:minute:seconds since last death
`ds1counter_{saveslot}_session_deaths.txt`			|	Number of deaths since this program was started (restart program for each session)
`ds1counter.json`										|	All above stats as a JSON formatted file
