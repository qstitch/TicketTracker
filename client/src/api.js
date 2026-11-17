const BASE_URL = import.meta.env.VITE_API_URL;

export async function getTickets() {
    const response = await fetch(`${BASE_URL}/api/tickets`);

    if (!response.ok) {
        throw new Error(`Request failed with status ${response.status}`);
    }
    return await response.json();
}