import React, { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import{ FiArrowLeft } from 'react-icons/fi';

import api from '../../services/api';

import './styles.css';

import logoImage from '../../assets/logo.svg';

export default function NewBook()
{

    const[id, setId] = useState(null);
    const[author, setAuthor] = useState('');
    const[title, setTitle] = useState('');
    const[launch, setLaunch] = useState('');
    const[price, setPrice] = useState('');

    const { bookId } = useParams();

    const navigate = useNavigate();

    const acessToken = localStorage.getItem('acessToken');

    const authorization = {
        headers: {
            Authorization: `Bearer ${acessToken}`
        }
    }

    useEffect(() => {
        if(bookId === '0') return;
        else loadBook();
    }, bookId);

    async function loadBook()
    {
        try {
            const response = await api.get(`api/book/v1/${bookId}`, authorization);

            let adjustedDate = response.data.launch.split("T", 10)[0];

            setId(response.data.id);
            setTitle(response.data.title);
            setAuthor(response.data.author);
            setPrice(response.data.price);
            setLaunch(adjustedDate);
        } catch (error) {
            
        }
    }

    async function saveOrUpdate(e){
        e.preventDefault();

        const data = {
            title,
            author,
            launch,
            price
        }

        try {
            if(bookId === '0'){
                await api.post('api/book/v1', data, authorization);
            }else {
                data.id = id;
                await api.put('api/book/v1', data, authorization);
            }
            navigate('/books');
        } catch (erro) {
            alert('Error while recordin Book! Try again!');
        }

    }

    return(
        <div className="new-book-container">
            <section className="form">
                <img src={logoImage} alt="Erudio"/>
                <h1>{bookId === '0' ? 'Add' : 'Update'} New Book</h1>
                <p>Enter the book information and click on {bookId === '0' ? `'Add'` : `'Update'`}!</p>
                <Link className="back-link" to='/books'>
                    <FiArrowLeft size={16} color='#251FC5' />
                    Back to Books
                </Link>
            </section>
            
            <form onSubmit={saveOrUpdate}>
                <input placeholder='Title' value={title} 
                    onChange={e => setTitle(e.target.value)} />
                <input placeholder='Author' value={author} 
                    onChange={e => setAuthor(e.target.value)} />
                <input type='date' value={launch} 
                    onChange={e => setLaunch(e.target.value)} />
                <input placeholder='Price' value={price} 
                    onChange={e => setPrice(e.target.value)} />

                <button className='button' type='submit'>{bookId === '0' ? 'Add' : 'Update'}</button>
            </form>

        </div>
    );
}