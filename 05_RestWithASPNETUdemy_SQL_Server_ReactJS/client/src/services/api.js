import axios from "axios";

const api = axios.create({
    baseURL : 'https://localhost:7070/'
})

export default api;