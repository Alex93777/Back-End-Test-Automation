import { expect } from "chai";
import { bookService } from "../functions/bookService.js";

describe("Book Service Tests", function () {

    describe("getBooks()", function () {
        // Test: Should return a status 200 and an array of books
        // 1. Verify that the response status is 200.
        // 2. Check that the first book includes the required keys: 'id', 'title', 'author', 'year', 'genre'.
        it("Should return a status 200 and an array of books", function () {
            const response = bookService.getBooks();

            expect(response.status).to.equal(200);
            expect(response.data).to.be.an('array').that.has.lengthOf(3);
            expect(response.data[0]).to.have.all.keys('id', 'title', 'author', 'year', 'genre');
            expect(response.data[1]).to.have.all.keys('id', 'title', 'author', 'year', 'genre');
            expect(response.data[2]).to.have.all.keys('id', 'title', 'author', 'year', 'genre');
        })
    });

    describe("addBook()", function () {
        // Test: Should add a new book successfully
        // 1. Create a new valid book object.
        // 2. Verify the response status is 201 and the success message is correct.
        // 3. Verify that the newly added book is present in the book list.
        it("Should add a new book successfully", function () {
            const newBook = {
                id: "4",
                title: "Test book",
                author: "Test author",
                year: 2020,
                genre: "Drama"
            }

            const response = bookService.addBook(newBook);
            expect(response.status).to.equal(201);
            expect(response.message).to.equal("Book added successfully.");

            const allBooks = bookService.getBooks().data;
            expect(allBooks).to.deep.include(newBook);
        })

        // Test: Should return status 400 when adding a book with missing fields
        // 1. Create an invalid book object with missing fields.
        // 2. Check if the response status is 400 and the error message is "Invalid Book Data!".
        it("Should return status 400 when adding a book with missing fields", function () {
            const newBook = {
                id: "4",
                title: "Test book"
            }

            const response = bookService.addBook(newBook);
            expect(response.status).to.equal(400);
            expect(response.error).to.equal("Invalid Book Data!");
        })
    });

    describe("deleteBook()", function () {
        // Test: Should delete a book by id successfully
        // 1. Add a book and then delete it by its ID.
        // 2. Verify the response status is 200 and the success message is correct.
        // 3. Ensure the book count returns the sum of the initial count of the books and the count of the added books from the tests
        it("Should delete a book by id successfully", function () {
            const bookToBeDeleted = "1";
            const response = bookService.deleteBook(bookToBeDeleted);

            expect(response.status).to.equal(200);
            expect(response.message).to.equal("Book deleted successfully.");

            const allBooks = bookService.getBooks().data;
            const foundBook = allBooks.filter(book => book.id === bookToBeDeleted);
            expect(foundBook.length).to.equal(0)
        })

        // Test: Should return status 404 when deleting a book with a non-existent id
        // 1. Attempt to delete a book with a non-existent ID.
        // 2. Check that the response status is 404 and the error message is "Book Not Found!".
        it("Should return status 404 when deleting a book with a non-existent id", function () {
            const bookToBeDeleted = "9";
            const response = bookService.deleteBook(bookToBeDeleted);

            expect(response.status).to.equal(404);
            expect(response.error).to.equal("Book Not Found!");
        })
    });

    describe("updateBook()", function () {
        // Test: Should update a book successfully
        // 1. Create updated data for an existing book.
        // 2. Verify the response status is 200 and the success message is correct.
        // 3. Ensure that the updated book fields reflect the new data.
        it("Should update a book successfully", function(){
            const oldId = "2";
            const newBook = {
                id: oldId,
                title: "To Kill a Mockingbird_2",
                author: "Harper Lee",
                year: 2020,
                genre: "Fiction"
            }

            const response = bookService.updateBook(oldId, newBook);
            expect(response.status).to.equal(200);
            expect(response.message).to.equal("Book updated successfully.");

            const allBooks = bookService.getBooks().data;
            expect(allBooks).to.deep.include(newBook);
        })

        // Test: Should return status 404 when updating a non-existent book
        // 1. Attempt to update a book that doesn't exist.
        // 2. Check that the response status is 404 and the error message is "Book Not Found!".
        it("Should return status 404 when updating a non-existent book", function() {
            const oldId = "9";
            const newBook = {
                id: oldId,
                title: "To Kill a Mockingbird_2",
                author: "Harper Lee",
                year: 2020,
                genre: "Fiction"
            }

            const response = bookService.updateBook(oldId, newBook);
            expect(response.status).to.equal(404);
            expect(response.error).to.equal("Book Not Found!");
        })

        // Test: Should return status 400 when updating with incomplete book data
        // 1. Provide an incomplete book object with missing fields.
        // 2. Verify that the response status is 400 and the error message is "Invalid Book Data!".
        it("Should return status 400 when updating with incomplete book data", function() {
            const oldId = "1";
            const newBook = {
                id: oldId,
                title: "To Kill a Mockingbird_2",
                author: "Test author",
            }

            const response = bookService.updateBook(oldId, newBook);
            expect(response.status).to.equal(400);
            expect(response.error).to.equal("Invalid Book Data!");
        })
    });
});