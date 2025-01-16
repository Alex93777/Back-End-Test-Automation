function solve(athletes) {
    function getAthletesBySport(sport) {
        const filteredAthletes = athletes.filter(athlete => athlete.sport === sport)

        return filteredAthletes;
    }
    function addAthlete(id, name, sport, medals, country) {
        const athleteToAdd = { id, name, sport, medals, country }
        athletes.push(athleteToAdd)

        return athletes;
    }

    function getAthleteById(id) {
        const foundAthlete = athletes.find(athletes => athletes.id === id)

        if (foundAthlete) {
            return foundAthlete
        }
        else {
            return `Athlete with ID ${id} not found`
        }

        //return foundAthlete ?? `Athlete with ID ${id} not found`        // с тернарен оператор
    }

    function removeAthleteById(id) {
        const initialLenght = athletes.length;

        athletes = athletes.filter(athletes => athletes.id !== id)

        if (initialLenght !== athletes.length) {
            return athletes;
        } else {
            return `Athlete with ID ${id} not found`
        }
    }

    function updateAthleteMedals(id, newMedals) {
        const foundAthletes = athletes.find(athlete => athlete.id === id);

        if(foundAthletes){
            foundAthletes.medals = newMedals;
            return athletes;
        } else {
            return `Athlete with ID ${id} not found`;
        }
    }

    function updateAthleteCountry(id, newCountry) {
        const foundAthletes = athletes.find(athlete => athlete.id === id);

        if(foundAthletes){
           foundAthletes.country = newCountry;
           return athletes;
        } else {
            return `Athlete with ID ${id} not found`;
        }
    }

    return {
        getAthletesBySport,
        addAthlete,
        getAthleteById,
        removeAthleteById,
        updateAthleteMedals,
        updateAthleteCountry
    };

}

let athletes = [
    { id: 1, name: "Usain Bolt", sport: "Sprinting", medals: 8, country: "Jamaica" },
    { id: 2, name: "Michael Phelps", sport: "Swimming", medals: 23, country: "USA" },
    { id: 3, name: "Simone Biles", sport: "Gymnastics", medals: 7, country: "USA" }
];


let solveFunctionResults = solve(athletes);

const results = solveFunctionResults.updateAthleteCountry(10, "Canada");

console.log(JSON.stringify(results));


