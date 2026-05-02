// js/find.js

document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('searchForm');
    const zipInput = document.getElementById('zipInput');
    const resultsDiv = document.getElementById('results');

    form.addEventListener('submit', async function (e) {
        e.preventDefault();
        const zip = zipInput.value.trim();
        resultsDiv.innerHTML = '';
        if (!zip) {
            resultsDiv.innerHTML = '<div class="alert alert-warning">Please enter a zip code.</div>';
            return;
        }
        resultsDiv.innerHTML = '<div class="text-center">Searching...</div>';
        try {
            const response = await fetch(`http://localhost:5000/api/restaurantapi/by-location?zip=${encodeURIComponent(zip)}`);
            if (!response.ok) throw new Error('API error');
            const data = await response.json();
            if (!data || !data.restaurants || data.restaurants.length === 0) {
                resultsDiv.innerHTML = '<div class="alert alert-info">No restaurants found for this zip code.</div>';
                return;
            }
            resultsDiv.innerHTML = renderRestaurants(data.restaurants);
        } catch (err) {
            resultsDiv.innerHTML = '<div class="alert alert-danger">Error fetching restaurants. Please try again later.</div>';
        }
    });
});

function renderRestaurants(restaurants) {
    return `
        <h3>Restaurants Found</h3>
        <div class="row">
            ${restaurants.map(r => `
                <div class="col-md-6 col-lg-4 mb-4">
                    <div class="card h-100">
                        <div class="card-body">
                            <h5 class="card-title">${r.restaurant_name || 'N/A'}</h5>
                            <p class="card-text">
                                <strong>Address:</strong> ${r.address || 'N/A'}<br/>
                                <strong>Cuisine:</strong> ${r.cuisine_type || 'N/A'}<br/>
                                <strong>Rating:</strong> ${r.rating || 'N/A'}
                            </p>
                        </div>
                    </div>
                </div>
            `).join('')}
        </div>
    `;
}
