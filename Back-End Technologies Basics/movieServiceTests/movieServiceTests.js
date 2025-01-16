import { expect } from "chai";
import { movieService } from "../functions/movieService.js";

describe("movieService Tests", function () {

    describe("getMovies()", function () {
        //// Test: Should return all movies with status 200
        // 1. Check if the response status is 200.
        // 2. Ensure the data is an array with a length of 3.
        // 3. Verify the first item contains the required keys: 'id', 'name', 'genre', 'year', 'director', 'rating', 'duration', 'language', 'desc'.
        it("should return successfull response with a list of 3 movies", function () {
            const response = movieService.getMovies();

            expect(response.status).to.equal(200);
            expect(response.data).to.be.an('array').that.has.lengthOf(3);
            expect(response.data[0]).to.have.all.keys('id', 'name', 'genre', 'year', 'director', 'rating', 'duration', 'language', 'desc');
            expect(response.data[1]).to.have.all.keys('id', 'name', 'genre', 'year', 'director', 'rating', 'duration', 'language', 'desc');
            expect(response.data[2]).to.have.all.keys('id', 'name', 'genre', 'year', 'director', 'rating', 'duration', 'language', 'desc');
        })
    });

    describe("addMovie()", function () {
        // Test: Should successfully add a new movie
        // 1. Create a new movie object with valid data.
        // 2. Check if the response status is 201 and the success message is correct.
        // 3. Verify that the newly added movie is present in the movie list. 
        it("should add new movie successfully", function () {
            const newMovie = {
                id: "4",
                name: "Test-Movie",
                genre: "Drama",
                year: 2010,
                director: "test director",
                rating: 9.8,
                duration: 90,
                language: "English, Bulgarian",
                desc: "Some description."
            }

            const response = movieService.addMovie(newMovie);
            expect(response.status).to.equal(201);
            expect(response.message).to.equal("Movie added successfully.");

            const allMovies = movieService.getMovies().data;
            expect(allMovies).to.deep.include(newMovie);
        })


        // Test: Should return an error for invalid movie data
        // 1. Create a movie object with incomplete or invalid data.
        // 2. Check if the response status is 400 and the error message is correct.

        it("should return an error for invalid movie data", function () {
            const newMovie = {
                id: "4",
                name: "Test-Movie",
                genre: "Drama",
            }

            const response = movieService.addMovie(newMovie);
            expect(response.status).to.equal(400);
            expect(response.error).to.equal("Invalid Movie Data!");
        })
    });

    describe('deleteMovie()', function () {
        // Test: Should delete a movie by id successfully
        // 1. Add a movie to ensure there is one to delete.
        // 2. Delete the movie by its id and check if the response status is 200.
        // 3. Verify that the success message is correct.
        // 4. Ensure that the movie is no longer in the list.
        it("should delete a movie by id successfully", function () {
            const movieIdToBeDeleted = "3";
            const response = movieService.deleteMovie(movieIdToBeDeleted);

            expect(response.status).to.equal(200);
            expect(response.message).to.equal("Movie deleted successfully.");

            const allMovies = movieService.getMovies().data;
            const foundMovie = allMovies.filter(movie => movie.id === movieIdToBeDeleted);
            expect(foundMovie.length).to.equal(0);
        })


        // Test: Should return 404 for a non-existent movie id
        // 1. Attempt to delete a movie with an id that doesn't exist.
        // 2. Check if the response status is 404 and the error message is correct.
        it("should return 404 for a non-existent movie id", function () {
            const movieIdToBeDeleted = "999";
            const response = movieService.deleteMovie(movieIdToBeDeleted);

            expect(response.status).to.equal(404);
            expect(response.error).to.equal("Movie Not Found!");
        })
    });

    describe("updateMovie()", function () {
        // Test: Should update an existing movie successfully
        // 1. Create an updated movie object with valid data.
        // 2. Update the movie by its name and check if the response status is 200.
        // 3. Verify that the success message is correct.
        // 4. Ensure that the updated movie is present in the movie list.
        it("should update an existing movie successfully", function () {
            const oldName = "Inception";
            const newMovie = {
                id: "1",
                name: "Inception_2",
                genre: "Drama",
                year: 2024,
                director: "Christopher Nolan",
                rating: 8.8,
                duration: 148,
                language: "English, Bulgarian",
                desc: "A thief who steals corporate secrets through the use of dream-sharing technology."
            }

            const response = movieService.updateMovie(oldName, newMovie);
            expect(response.status).to.equal(200);
            expect(response.message).to.equal("Movie updated successfully.");

            const allMovies = movieService.getMovies().data;
            expect(allMovies).to.deep.include(newMovie);
        })

        // Test: Should return an error if the movie to update does not exist
        // 1. Attempt to update a movie that doesn't exist.
        // 2. Check if the response status is 404 and the error message is correct.
        it("should return an error if the movie to update does not exist", function(){
            const oldName = "Not existing movie";
            const newMovie = {
                id: "1",
                name: "Inception_2",
                genre: "Drama",
                year: 2024,
                director: "Christopher Nolan",
                rating: 8.8,
                duration: 148,
                language: "English, Bulgarian",
                desc: "A thief who steals corporate secrets through the use of dream-sharing technology."
            }

            const response = movieService.updateMovie(oldName, newMovie);
            expect(response.status).to.equal(404);
            expect(response.error).to.equal("Movie Not Found!");
        })

        // Test: Should return an error if the new movie data is invalid
        // 1. Provide incomplete or invalid data for an existing movie.
        // 2. Check if the response status is 400 and the error message is correct.
        it("Should return an error if the new movie data is invalid", function(){
            const oldName = "The Matrix";
            const newMovie = {
                id: "2",
                name: "The Matrix_2",
            }

            const response = movieService.updateMovie(oldName, newMovie);
            expect(response.status).to.equal(400);
            expect(response.error).to.equal("Invalid Movie Data!");
        })
    });
});