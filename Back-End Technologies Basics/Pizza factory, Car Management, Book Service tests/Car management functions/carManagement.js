function solve(cars) {
    function getCarsByBrand(brand) {
        const filteredCars = cars.filter(cars => cars.brand === brand);

        return filteredCars;
    }

    function addCar(id, brand, model, year, price, inStock) {
        const carToAdd = { id, brand, model, year, price, inStock };

        cars.push(carToAdd);

        return cars;
    }

    function getCarById(id) {
        const foundCarById = cars.find(cars => cars.id === id);
        
        if(foundCarById){
            return foundCarById;
        } else {
            return `Car with ID ${id} not found`;
        }
    }

    function removeCarById(id) {
        const initialLength = cars.length;

        cars = cars.filter(cars => cars.id !== id);

        if(initialLength !== cars.length){
            return cars;
        } else {
            return `Car with ID ${id} not found`;
        }
    }

    function updateCarPrice(id, newPrice) {
        const carToUpdate = cars.find(cars => cars.id === id);

        if(carToUpdate){
            carToUpdate.price = newPrice;
            return cars;
        } else {
            return `Car with ID ${id} not found`;
        }
    }

    function updateCarStock(id, inStock) {
        const carToUpdate = cars.find(cars => cars.id === id);

        if(carToUpdate){
            carToUpdate.inStock = inStock;
            return cars;
        } else {
            return `Car with ID ${id} not found`;
        }
    }

    return {
        getCarsByBrand,
        addCar,
        getCarById,
        removeCarById,
        updateCarPrice,
        updateCarStock
    };
}


let cars = [
    { id: 1, brand: "Toyota", model: "Corolla", year: 2020, price: 20000, inStock: true },
    { id: 2, brand: "Honda", model: "Civic", year: 2019, price: 22000, inStock: true },
    { id: 3, brand: "Ford", model: "Mustang", year: 2021, price: 35000, inStock: false }
  ];


let solveFunctionResult = solve(cars);

const result = solveFunctionResult.updateCarStock(1, false);

console.log(JSON.stringify(result));
