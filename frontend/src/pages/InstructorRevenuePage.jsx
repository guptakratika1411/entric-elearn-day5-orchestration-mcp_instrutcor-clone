import { useEffect, useState } from 'react'
import { getInstructorRevenue } from '../api.js'

const money = (value) => `$${Number(value).toFixed(2)}`

export default function InstructorRevenuePage() {
  const [rows, setRows] = useState([])
  const [error, setError] = useState('')

  useEffect(() => {
    getInstructorRevenue()
      .then((data) => {
        setRows(data)
        setError('')
      })
      .catch((e) => setError(e.message))
  }, [])

  return (
    <>
      <h1>Instructor Revenue</h1>
      <p className="muted">Each instructor earns 70% of sales that were not refunded.</p>
      {error && <p className="message error">{error}</p>}
      <div className="panel">
        <table className="cart-table">
          <thead>
            <tr>
              <th>Instructor</th><th className="num">Gross sales</th>
              <th className="num">Refunds</th><th className="num">Instructor share</th>
            </tr>
          </thead>
          <tbody>
            {rows.map((r) => (
              <tr key={r.instructor}>
                <td>{r.instructor}</td>
                <td className="num">{money(r.grossSales)}</td>
                <td className="num">{money(r.refunds)}</td>
                <td className="num">{money(r.instructorShare)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </>
  )
}
