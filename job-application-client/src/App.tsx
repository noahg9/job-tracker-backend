import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import {
    getAllApplications,
    addApplication,
    deleteApplication,
    updateApplication,
} from "./api";
import type { JobApplication } from "./api";

const STATUS_OPTIONS = [
    { value: 0, label: "Applied", color: "#2196f3" },
    { value: 1, label: "Interview", color: "#00bcd4" },
    { value: 2, label: "Offer", color: "#ff9800" },
    { value: 3, label: "Rejected", color: "#f44336" },
    { value: 4, label: "Accepted", color: "#4caf50" },
];

function App() {
    const [apps, setApps] = useState<JobApplication[]>([]);
    const [company, setCompany] = useState("");
    const [role, setRole] = useState("");
    const [status, setStatus] = useState(0);
    const [appliedDate, setAppliedDate] = useState(
        new Date().toISOString().substring(0, 10)
    );

    const [editingId, setEditingId] = useState<number | null>(null);
    const [editCompany, setEditCompany] = useState("");
    const [editRole, setEditRole] = useState("");
    const [editStatus, setEditStatus] = useState(0);
    const [editAppliedDate, setEditAppliedDate] = useState("");

    useEffect(() => {
        getAllApplications().then(setApps);
    }, []);

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault();

        const newApp: JobApplication = {
            company,
            role,
            status,
            appliedDate: new Date(appliedDate).toISOString(),
            notes: "",
        };

        const saved = await addApplication(newApp);
        setApps([...apps, saved]);

        setCompany("");
        setRole("");
        setStatus(0);
        setAppliedDate(new Date().toISOString().substring(0, 10));
    };

    const handleDelete = async (id: number) => {
        if (!window.confirm("Are you sure you want to delete this application?")) return;
        await deleteApplication(id);
        setApps(apps.filter((app) => app.id !== id));
    };

    const startEditing = (app: JobApplication) => {
        setEditingId(app.id ?? null);
        setEditCompany(app.company);
        setEditRole(app.role);
        setEditStatus(app.status);
        setEditAppliedDate(app.appliedDate.substring(0, 10));
    };

    const cancelEditing = () => {
        setEditingId(null);
        setEditCompany("");
        setEditRole("");
        setEditStatus(0);
        setEditAppliedDate("");
    };

    const saveEdit = async () => {
        if (editingId === null) return;

        const updatedApp: JobApplication = {
            id: editingId,
            company: editCompany,
            role: editRole,
            status: editStatus,
            appliedDate: new Date(editAppliedDate).toISOString(),
            notes: "",
        };

        await updateApplication(editingId, updatedApp);
        setApps(apps.map((app) => (app.id === editingId ? updatedApp : app)));
        cancelEditing();
    };

    // Helper for status badge style
    function StatusBadge({ status }: { status: number }) {
        const option = STATUS_OPTIONS.find((opt) => opt.value === status);
        return (
            <span
                style={{
                    backgroundColor: option?.color ?? "#ccc",
                    color: "white",
                    padding: "0.2rem 0.6rem",
                    borderRadius: "12px",
                    fontSize: "0.8rem",
                    fontWeight: "600",
                    minWidth: "90px",
                    display: "inline-block",
                    textAlign: "center",
                }}
            >
                {option?.label}
            </span>
        );
    }

    return (
        <div
            style={{
                fontFamily: "'Segoe UI', Tahoma, Geneva, Verdana, sans-serif",
                padding: "2rem",
                maxWidth: "700px",
                margin: "0 auto",
                backgroundColor: "#f9f9f9",
                borderRadius: "12px",
                boxShadow: "0 4px 12px rgba(0,0,0,0.1)",
            }}
        >
            <h1
                style={{
                    textAlign: "center",
                    marginBottom: "1.5rem",
                    color: "#333",
                    fontWeight: "700",
                }}
            >
                Job Applications Tracker
            </h1>

            {/* Add Application Form */}
            <form
                onSubmit={handleSubmit}
                style={{
                    display: "grid",
                    gridTemplateColumns: "repeat(auto-fit, minmax(140px, 1fr))",
                    gap: "0.8rem",
                    marginBottom: "2rem",
                    alignItems: "center",
                }}
            >
                <label style={{ display: "flex", flexDirection: "column", fontSize: "0.9rem", color: "#555" }}>
                    Company
                    <input
                        placeholder="Company"
                        value={company}
                        onChange={(e) => setCompany(e.target.value)}
                        required
                        style={{
                            padding: "0.4rem 0.6rem",
                            fontSize: "1rem",
                            borderRadius: "6px",
                            border: "1px solid #ccc",
                            marginTop: "0.2rem",
                            outline: "none",
                            transition: "border-color 0.3s",
                        }}
                        onFocus={(e) => (e.target.style.borderColor = "#2196f3")}
                        onBlur={(e) => (e.target.style.borderColor = "#ccc")}
                    />
                </label>
                <label style={{ display: "flex", flexDirection: "column", fontSize: "0.9rem", color: "#555" }}>
                    Role
                    <input
                        placeholder="Role"
                        value={role}
                        onChange={(e) => setRole(e.target.value)}
                        required
                        style={{
                            padding: "0.4rem 0.6rem",
                            fontSize: "1rem",
                            borderRadius: "6px",
                            border: "1px solid #ccc",
                            marginTop: "0.2rem",
                            outline: "none",
                            transition: "border-color 0.3s",
                        }}
                        onFocus={(e) => (e.target.style.borderColor = "#2196f3")}
                        onBlur={(e) => (e.target.style.borderColor = "#ccc")}
                    />
                </label>

                <label style={{ display: "flex", flexDirection: "column", fontSize: "0.9rem", color: "#555" }}>
                    Status
                    <select
                        value={status}
                        onChange={(e) => setStatus(Number(e.target.value))}
                        style={{
                            padding: "0.4rem 0.6rem",
                            fontSize: "1rem",
                            borderRadius: "6px",
                            border: "1px solid #ccc",
                            marginTop: "0.2rem",
                            outline: "none",
                            transition: "border-color 0.3s",
                        }}
                        onFocus={(e) => (e.target.style.borderColor = "#2196f3")}
                        onBlur={(e) => (e.target.style.borderColor = "#ccc")}
                    >
                        {STATUS_OPTIONS.map((opt) => (
                            <option key={opt.value} value={opt.value}>
                                {opt.label}
                            </option>
                        ))}
                    </select>
                </label>

                <label style={{ display: "flex", flexDirection: "column", fontSize: "0.9rem", color: "#555" }}>
                    Applied Date
                    <input
                        type="date"
                        value={appliedDate}
                        onChange={(e) => setAppliedDate(e.target.value)}
                        style={{
                            padding: "0.4rem 0.6rem",
                            fontSize: "1rem",
                            borderRadius: "6px",
                            border: "1px solid #ccc",
                            marginTop: "0.2rem",
                            outline: "none",
                            transition: "border-color 0.3s",
                        }}
                        onFocus={(e) => (e.target.style.borderColor = "#2196f3")}
                        onBlur={(e) => (e.target.style.borderColor = "#ccc")}
                    />
                </label>

                <button
                    type="submit"
                    style={{
                        gridColumn: "span 2",
                        backgroundColor: "#2196f3",
                        color: "white",
                        border: "none",
                        borderRadius: "6px",
                        padding: "0.6rem",
                        fontSize: "1.1rem",
                        fontWeight: "600",
                        cursor: "pointer",
                        transition: "background-color 0.3s",
                    }}
                    onMouseEnter={e => (e.currentTarget.style.backgroundColor = "#1976d2")}
                    onMouseLeave={e => (e.currentTarget.style.backgroundColor = "#2196f3")}
                >
                    Add Application
                </button>
            </form>

            {/* List of Applications */}
            <ul style={{ listStyle: "none", padding: 0, margin: 0 }}>
                {apps.map((app) =>
                    editingId === app.id ? (
                        <li
                            key={app.id}
                            style={{
                                backgroundColor: "white",
                                padding: "1rem",
                                marginBottom: "0.8rem",
                                borderRadius: "8px",
                                boxShadow: "0 1px 6px rgba(0,0,0,0.1)",
                                display: "grid",
                                gridTemplateColumns: "1fr 1fr 1fr 80px 80px",
                                gap: "0.6rem",
                                alignItems: "center",
                            }}
                        >
                            <input
                                value={editCompany}
                                onChange={(e) => setEditCompany(e.target.value)}
                                style={{ padding: "0.3rem 0.5rem", borderRadius: "4px", border: "1px solid #ccc" }}
                            />
                            <input
                                value={editRole}
                                onChange={(e) => setEditRole(e.target.value)}
                                style={{ padding: "0.3rem 0.5rem", borderRadius: "4px", border: "1px solid #ccc" }}
                            />
                            <select
                                value={editStatus}
                                onChange={(e) => setEditStatus(Number(e.target.value))}
                                style={{ padding: "0.3rem 0.5rem", borderRadius: "4px", border: "1px solid #ccc" }}
                            >
                                {STATUS_OPTIONS.map((opt) => (
                                    <option key={opt.value} value={opt.value}>
                                        {opt.label}
                                    </option>
                                ))}
                            </select>
                            <input
                                type="date"
                                value={editAppliedDate}
                                onChange={(e) => setEditAppliedDate(e.target.value)}
                                style={{ padding: "0.3rem 0.5rem", borderRadius: "4px", border: "1px solid #ccc" }}
                            />

                            <button
                                onClick={saveEdit}
                                style={{
                                    backgroundColor: "#4caf50",
                                    color: "white",
                                    border: "none",
                                    borderRadius: "4px",
                                    padding: "0.4rem 0.6rem",
                                    cursor: "pointer",
                                }}
                            >
                                Save
                            </button>
                            <button
                                onClick={cancelEditing}
                                style={{
                                    backgroundColor: "#f44336",
                                    color: "white",
                                    border: "none",
                                    borderRadius: "4px",
                                    padding: "0.4rem 0.6rem",
                                    cursor: "pointer",
                                }}
                            >
                                Cancel
                            </button>
                        </li>
                    ) : (
                        <li
                            key={app.id}
                            style={{
                                backgroundColor: "white",
                                padding: "1rem",
                                marginBottom: "0.8rem",
                                borderRadius: "8px",
                                boxShadow: "0 1px 6px rgba(0,0,0,0.1)",
                                display: "grid",
                                gridTemplateColumns: "2fr 2fr 1fr 1fr 80px 80px",
                                gap: "0.6rem",
                                alignItems: "center",
                                cursor: "default",
                            }}
                        >
                            <span>{app.company}</span>
                            <span>{app.role}</span>
                            <StatusBadge status={app.status} />
                            <span>{new Date(app.appliedDate).toLocaleDateString()}</span>
                            <button
                                onClick={() => startEditing(app)}
                                style={{
                                    backgroundColor: "#2196f3",
                                    color: "white",
                                    border: "none",
                                    borderRadius: "4px",
                                    padding: "0.4rem 0.6rem",
                                    cursor: "pointer",
                                }}
                            >
                                Edit
                            </button>
                            <button
                                onClick={() => app.id && handleDelete(app.id)}
                                style={{
                                    backgroundColor: "#f44336",
                                    color: "white",
                                    border: "none",
                                    borderRadius: "4px",
                                    padding: "0.4rem 0.6rem",
                                    cursor: "pointer",
                                }}
                            >
                                Delete
                            </button>
                        </li>
                    )
                )}
            </ul>
        </div>
    );
}

export default App;

