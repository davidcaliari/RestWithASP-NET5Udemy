import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { FiPower, FiEdit, FiTrash2 } from 'react-icons/fi';

import api from '../../services/api';

import './styles.css';

import logoImage from '../../assets/logo.svg';

export default function Books()
{

    const[books, setBooks] = useState([]);
    const[page, setPage] = useState(0);

    const userName = localStorage.getItem('userName');

    const acessToken = localStorage.getItem('acessToken');

    const authorization = {
        headers: {
            Authorization: `Bearer ${acessToken}`
        }
    }

    const navigate = useNavigate();

    useEffect(() => {
        fetchMoreBooks();
    }, [acessToken]);

    async function fetchMoreBooks(){
        const response = await api.get(`api/book/v1/asc/5/${page}`, authorization);
            setBooks([ ...books, ...response.data.list]);
            setPage(page + 1)
    };

    async function logout() {
        try {
            await api.get('api/auth/v1/revoke', authorization);

            localStorage.clear();
            navigate('/')
        } catch (erro) {
            alert(`Logout failed! Try again! ${erro}`);
        }
    }

    async function editBook(id){
        try {
            navigate(`/book/new/${id}`);
        } catch (erro) {
            alert('Edit book failed! Try again!');
        }
    }

    async function deleteBook(id){
        try {
            await api.delete(`api/book/v1/${id}`, authorization);

            setBooks(books.filter(book => book.id !== id));
        } catch (erro) {
            alert('Delete failed! Try again!');
        }
    }

    return (
        <div className="book-container">
            <header>
                <img src={logoImage} alt='Erudio' />
                <span>Welcome, <strong>{userName.toLowerCase()}</strong>!</span>
                <Link className='button' to='/book/new/0'>Add New Book</Link>
                
                <button onClick={logout} type='button'>
                    <FiPower size={18} color='#251FCS' />
                </button>
            </header>
            <ul>
                {books.map(book => (
                    <li key={book.id}>
                        <strong>Title:</strong>
                        <p>{book.title}</p>
                        <strong>Author:</strong>
                        <p>{book.author}</p>
                        <strong>Price:</strong>
                        <p>{Intl.NumberFormat('pt-BR', {style: 'currency', currency: 'BRL'}).format(book.price)}</p>
                        <strong>Release Date:</strong>
                        <p>{Intl.DateTimeFormat('pt-BR').format(new Date(book.launch))}</p>

                        <button onClick={() => editBook(book.id)} type='button'>
                            <FiEdit size={20} color='#251FCS' />
                        </button>

                        <button onClick={() => deleteBook(book.id)} type='button'>
                            <FiTrash2 size={20} color='#251FCS' />
                        </button>
                    </li>
                ))}
            </ul>
            <button className='button' onClick={fetchMoreBooks} type='button'>Load More</button>
        </div>
    );
}