function solve(products) {
    function getProductsByCategory(category) {
        const productsByCategory = products.filter(products => products.category === category);

        return productsByCategory;
    }

    function addProduct(id, name, category, price, stock) {
        const productToAdd = { id, name, category, price, stock };
        products.push(productToAdd)

        return products;
    }

    function getProductById(id) {
        const foundProduct = products.find(products => products.id === id);

        if (foundProduct) {
            return foundProduct;
        } else {
            return `Product with ID ${id} not found`;
        }
    }

    function removeProductById(id) {
        const initialLength = products.length;

        products = products.filter(products => products.id !== id);

        if (initialLength !== products.length) {
            return products;
        } else {
            return `Product with ID ${id} not found`;
        }
    }

    function updateProductPrice(id, newPrice) {
        const foundProduct = products.find(products => products.id === id);

        if (foundProduct) {
            foundProduct.price = newPrice;
            return products;
        } else {
            return `Product with ID ${id} not found`;
        }
    }

    function updateProductStock(id, newStock) {
        const foundProduct = products.find(products => products.id === id);

        if (foundProduct) {
            foundProduct.stock = newStock;
            return products;
        } else {
            return `Product with ID ${id} not found`;
        }
    }

    return {
        getProductsByCategory,
        addProduct,
        getProductById,
        removeProductById,
        updateProductPrice,
        updateProductStock
    };
}

const products = [
    { id: 1, name: "Laptop", category: "Electronics", price: 1200, stock: 30 },
    { id: 2, name: "Smartphone", category: "Electronics", price: 800, stock: 50 },
    { id: 3, name: "Headphones", category: "Accessories", price: 150, stock: 100 }
];


let solveFunctionResult = solve(products);

const result = solveFunctionResult.updateProductStock(2, 70);

console.log(JSON.stringify(result));
